import { create, ev } from '@open-wa/wa-automate';
import * as fs from 'fs';
import * as path from 'path';
// @ts-ignore
import heicConvert from 'heic-convert';
import sharp from 'sharp';

// Load configuration dynamically from working directory
const configPath = path.join(process.cwd(), 'config.js');
if (!fs.existsSync(configPath)) {
    console.error(`[!] Configuration file not found at: ${configPath}`);
    console.error(`[!] Please ensure config.js exists in the application directory.`);
    process.exit(1);
}
const config = require(configPath);

// ========================================
// CONFIGURATION
// ========================================
const ALLOWED_NUMBERS = config.ALLOWED_NUMBERS;
const DOWNLOADS_DIR = path.join(process.cwd(), config.FOLDER_SETTINGS.downloadsDir);

// ========================================
// STATE MANAGEMENT
// ========================================
let currentFolder: string | null = null;           // Current active folder path (Category/PhoneNumber)
let pendingPhoneNumber: string | null = null;      // Stores phone number while waiting for category selection
let mainAccountId: string | null = null;           // Main WhatsApp account ID
let photoCounters: { [key: string]: number } = {}; // Track photo count per folder

// ========================================
// CATEGORY CONFIGURATION
// ========================================
// Photo categories from config - these folders will be auto-created
const SUBFOLDER_OPTIONS = config.PHOTO_CATEGORIES;

// ========================================
// INITIALIZATION
// ========================================
// Create base category folders on startup if they don't exist
SUBFOLDER_OPTIONS.forEach((subfolder: string) => {
    const subfolderPath = path.join(DOWNLOADS_DIR, subfolder);
    if (!fs.existsSync(subfolderPath)) {
        fs.mkdirSync(subfolderPath, { recursive: true });
        console.log(`[+] Created base category folder: ${subfolderPath}`);
    }
});

/**
 * Get the current photo count for a folder
 * Counts only image files (jpg, jpeg, png, gif, webp)
 */
function getPhotoCount(folderPath: string): number {
    const fullPath = path.join(DOWNLOADS_DIR, folderPath);
    if (!fs.existsSync(fullPath)) return 0;

    const files = fs.readdirSync(fullPath);
    const imageExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp'];
    const photoCount = files.filter(file => {
        const ext = path.extname(file).toLowerCase();
        return imageExtensions.includes(ext);
    }).length;

    return photoCount;
}

// ========================================
// QR CODE EVENT HANDLER
// ========================================
// Listen for QR code generation and save as image file for scanning
ev.on('qr.**', async (qrcode, sessionId) => {
    const base64Data = qrcode.replace('data:image/png;base64,', '');
    const filename = `qr_code${sessionId ? '_' + sessionId : ''}.png`;
    try {
        const inputBuffer = Buffer.from(base64Data, 'base64');
        const blackAndWhiteBuffer = await sharp(inputBuffer)
            .flatten({ background: '#ffffff' })
            .threshold(200)
            .toColourspace('srgb')
            .png()
            .toBuffer();
        fs.writeFileSync(filename, blackAndWhiteBuffer);
    } catch (err) {
        console.warn(`[!] QR recoloring fallback: ${(err as Error)?.message}`);
        fs.writeFileSync(filename, base64Data, 'base64');
    }
    console.log(`[+] QR code saved as: ${filename}`);
    console.log(`[i] You can now scan the QR code from the image file: ${filename}`);
});

// ========================================
// MAIN BOT LOGIC
// ========================================
create(config.BOT_CONFIG).then(async client => {
    // Get main account information
    const me = await client.getMe();
    mainAccountId = me._serialized;
    console.log(`[+] Main account: ${me.pushname || me.name || me.id._serialized}`);
    console.log(`[+] Authorized numbers: ${ALLOWED_NUMBERS.join(', ')}`);
    console.log(`[+] Photo categories: ${SUBFOLDER_OPTIONS.join(', ')}`);

    // ========================================
    // MESSAGE HANDLER
    // ========================================
    client.onMessage(async message => {
        const senderNumber = message.from.replace('@c.us', '');
        const senderName = message.sender?.pushname || message.sender?.name || senderNumber;

        // ========================================
        // AUTHORIZATION CHECK
        // ========================================
        // Only allowed numbers can use this service
        if (!ALLOWED_NUMBERS.includes(senderNumber)) {
            console.log(`[!] Unauthorized access attempt from: ${senderNumber} (${senderName})`);
            await client.sendText(message.from, config.MESSAGES.unauthorized);
            return;
        }

        // ========================================
        // STEP 1: PHONE NUMBER INPUT
        // ========================================
        // User sends a phone number (7-15 digits)
        // Format: Clean number without special characters
        const cleanNumber = message.body.trim().replace(/[+\-\s\(\)]/g, '');
        if (message.body && /^[0-9]{7,15}$/.test(cleanNumber)) {
            const phoneNumber = cleanNumber;

            // Store phone number and prompt for category selection
            pendingPhoneNumber = phoneNumber;

            const categoryPrompt = config.MESSAGES.selectCategory
                .replace('{phone}', phoneNumber)
                .replace('{options}', SUBFOLDER_OPTIONS.map((opt: string, idx: number) => `${idx + 1}. ${opt}`).join('\n'));

            await client.sendText(message.from, categoryPrompt);
            console.log(`[i] Phone number received: ${phoneNumber}, awaiting category selection...`);
            return;
        }

        // ========================================
        // STEP 2: CATEGORY SELECTION
        // ========================================
        // User selects category (1-N) after sending phone number
        const categoryRegex = new RegExp(`^[1-${SUBFOLDER_OPTIONS.length}]$`);
        if (pendingPhoneNumber && message.body && categoryRegex.test(message.body.trim())) {
            const selection = parseInt(message.body.trim());
            const subfolder = SUBFOLDER_OPTIONS[selection - 1];
            const phoneNumber = pendingPhoneNumber;

            // Create folder structure: downloads/Category/PhoneNumber
            currentFolder = path.join(subfolder, phoneNumber);
            const folderPath = path.join(DOWNLOADS_DIR, currentFolder);

            // Create folder if it doesn't exist
            if (!fs.existsSync(folderPath)) {
                fs.mkdirSync(folderPath, { recursive: true });
                console.log(`[+] Created folder: ${folderPath}`);
            } else {
                console.log(`[i] Folder already exists: ${folderPath}`);
            }

            // Initialize photo counter for this folder (start from current count)
            photoCounters[currentFolder] = getPhotoCount(currentFolder);

            // Log folder creation
            const logMessage = `📱 **New folder activated**\nFrom: ${senderName} (${senderNumber})\nPhone: ${phoneNumber}\nCategory: ${subfolder}\nPath: ${currentFolder}\nExisting photos: ${photoCounters[currentFolder]}\nTime: ${new Date().toLocaleString()}`;
            console.log(logMessage);

            // Send confirmation with existing photo count
            const existingCount = photoCounters[currentFolder];
            await client.sendText(message.from,
                config.MESSAGES.folderCreated
                    .replace('{phone}', phoneNumber)
                    .replace('{category}', subfolder)
                    .replace('{count}', existingCount > 0 ? `\n📊 Existing photos in folder: ${existingCount}` : '')
            );

            pendingPhoneNumber = null; // Clear pending state
            return;
        }

        // ========================================
        // ERROR: CATEGORY SENT WITHOUT PHONE NUMBER
        // ========================================
        // User tries to select category before sending phone number
        const errorCategoryRegex = new RegExp(`^[1-${SUBFOLDER_OPTIONS.length}]$`);
        if (!pendingPhoneNumber && message.body && errorCategoryRegex.test(message.body.trim())) {
            await client.sendText(message.from, config.MESSAGES.phoneNumberFirst);
            return;
        }

        // ========================================
        // STEP 3: MEDIA HANDLING
        // ========================================
        // Process images, videos, and documents
        if (message.mimetype && (message.type === 'image' || message.type === 'video' || message.type === 'document')) {
            // Check if folder is set
            if (!currentFolder) {
                console.log('[!] No folder set. User must send phone number first.');
                await client.sendText(message.from, config.MESSAGES.noFolderSet);
                return;
            }

            try {
                // Decrypt and retrieve media data
                const mediaData = await client.decryptMedia(message);
                const timestamp = Date.now();

                // ========================================
                // FILE TYPE DETECTION
                // ========================================
                let fileExtension = 'jpg';
                let isImageDocument = false;

                // Check if document is actually an image
                if (message.type === 'document' && message.mimetype && message.mimetype.startsWith('image/')) {
                    isImageDocument = true;
                }

                // Determine file extension based on type
                if (message.type === 'video') {
                    if (message.mimetype.includes('mp4')) fileExtension = 'mp4';
                    else if (message.mimetype.includes('avi')) fileExtension = 'avi';
                    else if (message.mimetype.includes('mov')) fileExtension = 'mov';
                    else fileExtension = 'mp4';
                } else if (message.type === 'image' || isImageDocument) {
                    // All images are converted to JPG for consistency
                    fileExtension = 'jpg';
                } else if (message.type === 'document' && !isImageDocument) {
                    if (message.mimetype.includes('pdf')) fileExtension = 'pdf';
                    else if (message.mimetype.includes('doc')) fileExtension = 'doc';
                    else if (message.mimetype.includes('docx')) fileExtension = 'docx';
                    else if (message.mimetype.includes('txt')) fileExtension = 'txt';
                    else fileExtension = 'bin';
                }

                const fileName = `${timestamp}.${fileExtension}`;
                const filePath = path.join(DOWNLOADS_DIR, currentFolder, fileName);

                // ========================================
                // BUFFER CONVERSION
                // ========================================
                // Convert base64 data to buffer
                let buffer: Buffer;
                if (/^data:/.test(mediaData)) {
                    buffer = Buffer.from(mediaData.split(',')[1], 'base64');
                } else {
                    buffer = Buffer.from(mediaData, 'base64');
                }

                // ========================================
                // IMAGE PROCESSING & CONVERSION
                // ========================================

                // Convert HEIC/HEIF to JPEG
                if ((message.mimetype && (message.mimetype.includes('heic') || message.mimetype.includes('heif'))) ||
                    fileExtension === 'heic' || fileExtension === 'heif') {
                    try {
                        const outputBuffer = await heicConvert({
                            buffer,
                            format: 'JPEG',
                            quality: 1 // Maximum quality
                        });
                        fileExtension = 'jpg';
                        fs.writeFileSync(filePath.replace(/\.[^.]+$/, '.jpg'), outputBuffer as any);
                        buffer = outputBuffer as Buffer;
                    } catch (err) {
                        console.error('[!] HEIC/HEIF conversion failed:', err);
                        await client.sendText(message.from, config.MESSAGES.conversionError);
                        return;
                    }
                }
                // Re-encode JPEG with sharp for consistency
                else if (fileExtension === 'jpg' || fileExtension === 'jpeg') {
                    try {
                        const outputBuffer = await sharp(buffer).jpeg({ quality: 100 }).toBuffer();
                        fs.writeFileSync(filePath, outputBuffer as any);
                        buffer = outputBuffer as Buffer;
                    } catch (err) {
                        // Fallback to original buffer if sharp fails
                        fs.writeFileSync(filePath, buffer as any);
                    }
                }
                // Save other file types as-is
                else {
                    fs.writeFileSync(filePath, buffer as any);
                }

                // ========================================
                // PHOTO COUNTER & CONFIRMATION
                // ========================================
                const fileSize = (buffer.length / 1024).toFixed(2);
                let mediaType, messageTemplate;
                let isPhoto = false;
                let totalPhotoCount = 0;

                if (message.type === 'image' || isImageDocument) {
                    mediaType = '📸 Photo';
                    messageTemplate = config.MESSAGES.photoSaved;
                    isPhoto = true;

                    // Get total photo count from folder after saving
                    totalPhotoCount = getPhotoCount(currentFolder);
                } else if (message.type === 'video') {
                    mediaType = '🎥 Video';
                    messageTemplate = config.MESSAGES.videoSaved;
                } else {
                    mediaType = '📄 Document';
                    messageTemplate = config.MESSAGES.documentSaved;
                }

                // Log media receipt
                const logMessage = `${mediaType} received\nFrom: ${senderName} (${senderNumber})\nFolder: ${currentFolder}\nFile: ${fileName}\nSize: ${fileSize} KB${isPhoto ? `\nTotal Photos: ${totalPhotoCount}` : ''}\nTime: ${new Date().toLocaleString()}`;
                console.log(logMessage);

                // Send confirmation message with total photo count
                const confirmationMessage = messageTemplate
                    .replace('{folder}', currentFolder)
                    .replace('{filename}', fileName)
                    .replace('{count}', isPhoto ? totalPhotoCount.toString() : '')
                    .replace('{size}', fileSize);

                await client.sendText(message.from, confirmationMessage);

            } catch (error) {
                console.error(`[!] Error saving media: ${error instanceof Error ? error.message : String(error)}`);
                await client.sendText(message.from, config.MESSAGES.errorSaving);
            }
        }

        // ========================================
        // HELP & INSTRUCTIONS
        // ========================================
        // Show instructions for any unrecognized message
        const cleanMessageNumber = message.body.trim().replace(/[+\-\s\(\)]/g, '');
        const helpCategoryRegex = new RegExp(`^[1-${SUBFOLDER_OPTIONS.length}]$`);
        if (message.body && !/^[0-9]{7,15}$/.test(cleanMessageNumber) &&
            message.type !== 'image' && message.type !== 'video' && message.type !== 'document' &&
            !helpCategoryRegex.test(message.body.trim())) {

            const currentFolderInfo = currentFolder
                ? `${currentFolder} (${photoCounters[currentFolder] || 0} photos)`
                : 'None';

            const instructionsMessage = config.MESSAGES.instructions
                .replace('{currentFolder}', currentFolderInfo)
                .replace('{pendingPhone}', pendingPhoneNumber ? `\n⏳ Awaiting category selection for: ${pendingPhoneNumber}` : '');
            await client.sendText(message.from, instructionsMessage);
        }
    });
});