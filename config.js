// ========================================
// WHATSAPP PHOTO MANAGER CONFIGURATION
// ========================================
// Professional photo management system for WhatsApp
// Edit this file to customize your bot settings

const fs = require('fs');
const path = require('path');

// Helper to find Google Chrome bundled inside the local 'chrome' folder
function findBundledBrowser() {
    const chromeDir = path.join(process.cwd(), 'chrome');
    if (!fs.existsSync(chromeDir)) return undefined;

    // Recursively search for chrome.exe
    const searchForChrome = (dir) => {
        try {
            const files = fs.readdirSync(dir);
            for (const file of files) {
                const fullPath = path.join(dir, file);
                const stat = fs.statSync(fullPath);
                if (stat.isDirectory()) {
                    const found = searchForChrome(fullPath);
                    if (found) return found;
                } else if (file.toLowerCase() === 'chrome.exe') {
                    return fullPath;
                }
            }
        } catch (e) {
            // Ignore directory read errors
        }
        return null;
    };
    
    return searchForChrome(chromeDir) || undefined;
}

const defaultConfig = {
    // ========================================
    // AUTHORIZED PHONE NUMBERS
    // ========================================
    // Define which phone numbers are allowed to use this service
    // Format: Number only, without + or country code prefix
    // Examples: '1234567890', '9779800000000', '919876543210'
    
    ALLOWED_NUMBERS: [
        // ⬇️ ADD YOUR AUTHORIZED PHONE NUMBERS HERE ⬇️
        // '1234567890',
        // '9779800000000',
    ],

    // ========================================
    // BOT CONFIGURATION
    // ========================================
    // Core WhatsApp bot settings and session management
    
    BOT_CONFIG: {
        sessionId: 'photo-manager-session',      // Unique session identifier
        useChrome: true,                         // Recommended by wa-automate for reliable multi-device support
        authTimeout: 0,                          // 0 = Wait indefinitely for scan/auth without abrupt timeout disconnect
        qrTimeout: 0,                            // 0 = Wait indefinitely for QR generation
        protocolTimeout: 0,                      // 0 = Wait indefinitely for CDP commands during heavy multi-device chat sync
        multiDevice: true,                       // Enable WhatsApp multi-device support
        safeMode: true,                          // Official wa-automate safeMode for reliable injection
        headless: true,                          // Run browser in background (set false to debug or re-scan QR)
        deleteSessionDataOnLogout: false,        // Keep session data intact; avoids wa-automate eventMode deadlock
        killClientOnLogout: false,               // Prevents wa-automate forcing eventMode
        eventMode: false,                        // Register listeners on-demand (e.g. onMessage) instead of exposing 27 listeners simultaneously
        waitForRipeSession: true,                // Wait for session to be fully ready before injection
        restartOnCrash: true,                    // Auto-restart browser on unexpected page termination
        inDocker: true,                          // Activates native customUserAgent support in wa-automate without node_modules patches
        sessionDataPath: '.',                    // Store session data in application working directory
        customUserAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36', // Modern Chrome user agent supported natively by wa-automate
    },

    // ========================================
    // FOLDER & FILE SETTINGS
    // ========================================
    // Configure download directory and file handling
    
    FOLDER_SETTINGS: {
        downloadsDir: 'downloads',               // Root directory for all downloads
        maxFileSize: 50 * 1024 * 1024,          // Maximum file size: 50MB
    },

    // ========================================
    // PHOTO CATEGORIES
    // ========================================
    // Define photo categories - these folders will be auto-created
    // Users will select from these categories when organizing photos
    
    PHOTO_CATEGORIES: [
        'Photo Category 1',
        'Photo Category 2', 
        'Photo Category 3',
        'Photo Category 4'
    ],

    // ========================================
    // BOT MESSAGES & RESPONSES
    // ========================================
    // Customize all bot responses and user-facing messages
    // Variables: {phone}, {category}, {folder}, {filename}, {count}, {size}
    
    MESSAGES: {
        // Authorization Messages
        unauthorized: 
`🚫 **Access Denied**

You are not authorized to use this service. Please contact the administrator for access.`,

        // Category Selection
        selectCategory: 
`📂 **Select Photo Category**
━━━━━━━━━━━━━━━━━━━━━━━━
📱 Phone Number: *{phone}*

Please choose the photo category:
{options}

💡 Reply with the number (1-4) to continue.`,

        // Error: Wrong Order
        phoneNumberFirst: 
`⚠️ **Incorrect Order**
━━━━━━━━━━━━━━━━━━━━━━━━

Please follow the correct workflow:

**Step 1:** Send phone number (7-15 digits)
**Step 2:** Select category (1-N)

📝 Example: First send "1234567890", then send "2"`,

        // Folder Activation Success
        folderCreated: 
`✅ **Folder Activated Successfully**
━━━━━━━━━━━━━━━━━━━━━━━━
📱 Phone: *{phone}*
📂 Category: *{category}*{count}

🎯 Ready to receive photos! Send images now.`,

        // Photo Saved
        photoSaved: 
`✅ Photo saved successfully in folder "{folder}" as {filename}
Total: {count}`,

        // Video Saved
        videoSaved: 
`✅ Video saved successfully in folder "{folder}" as {filename}`,

        // Document Saved
        documentSaved: 
`✅ Document saved successfully in folder "{folder}" as {filename}`,

        // Error: No Folder Set
        noFolderSet: 
`❌ **No Active Folder**
━━━━━━━━━━━━━━━━━━━━━━━━

Please set up a folder first:

**Step 1:** Send phone number (7-15 digits)
**Step 2:** Select category (1-N)
**Step 3:** Send photos

💡 Type "help" for detailed instructions.`,

        // Error: Saving Failed
        errorSaving: 
`❌ **Save Failed**
━━━━━━━━━━━━━━━━━━━━━━━━

Unable to save the file. Please try again.

If the problem persists:
• Check file size (max 50MB)
• Ensure file is not corrupted
• Contact administrator if issue continues`,

        // Error: Conversion Failed
        conversionError: 
`❌ **Image Conversion Failed**
━━━━━━━━━━━━━━━━━━━━━━━━

Unable to convert HEIC/HEIF image format.

Please try:
• Converting to JPG on your device
• Sending as a different format
• Sending as a document instead`,

        // Instructions & Help
        instructions: 
`📖 **Photo Manager - User Guide**
━━━━━━━━━━━━━━━━━━━━━━━━

**🔄 Workflow:**

**Step 1:** Send Phone Number
   • Format: 7-15 digits
   • Example: 1234567890

**Step 2:** Select Category
   1️⃣ Photo Category 1
   2️⃣ Photo Category 2
   3️⃣ Photo Category 3
   4️⃣ Photo Category 4

**Step 3:** Upload Media
   • Photos (JPG, PNG, HEIC)
   • Videos (MP4, AVI, MOV)
   • Documents (PDF, DOC, DOCX)

━━━━━━━━━━━━━━━━━━━━━━━━
📊 **Current Status:**
📂 Active Folder: {currentFolder}{pendingPhone}

💡 **Tips:**
• All images auto-convert to JPG
• Maximum file size: 50MB
• Photos are numbered automatically
• HEIC/HEIF formats supported

━━━━━━━━━━━━━━━━━━━━━━━━
Need help? Contact administrator.`,
    }
};

// Start with default configuration
let config = { ...defaultConfig };

// Add the bundled browser if found
const bundledChrome = findBundledBrowser();
if (bundledChrome) {
    config.BOT_CONFIG.executablePath = bundledChrome;
    console.log(`[+] Using bundled Chrome browser: ${bundledChrome}`);
}

// Load overrides from config.json if exists in current working directory
const jsonConfigPath = path.join(process.cwd(), 'config.json');
if (fs.existsSync(jsonConfigPath)) {
    try {
        const fileContent = fs.readFileSync(jsonConfigPath, 'utf8').replace(/^\uFEFF/, '');
        const overrides = JSON.parse(fileContent);
        
        if (overrides.ALLOWED_NUMBERS) {
            config.ALLOWED_NUMBERS = overrides.ALLOWED_NUMBERS;
        }
        if (overrides.PHOTO_CATEGORIES) {
            config.PHOTO_CATEGORIES = overrides.PHOTO_CATEGORIES;
        }
        if (overrides.FOLDER_SETTINGS) {
            config.FOLDER_SETTINGS = {
                ...config.FOLDER_SETTINGS,
                ...overrides.FOLDER_SETTINGS
            };
        }
        if (overrides.BOT_CONFIG) {
            config.BOT_CONFIG = {
                ...config.BOT_CONFIG,
                ...overrides.BOT_CONFIG
            };
        }
        
        // Respect explicit executablePath overrides in JSON, else use bundled
        if (overrides.BOT_CONFIG && overrides.BOT_CONFIG.executablePath) {
            config.BOT_CONFIG.executablePath = overrides.BOT_CONFIG.executablePath;
        } else if (bundledChrome) {
            config.BOT_CONFIG.executablePath = bundledChrome;
        }
        
        console.log(`[+] Loaded configuration overrides from ${jsonConfigPath}`);
    } catch (e) {
        console.error(`[!] Failed to parse config.json: ${e.message}`);
    }
}

module.exports = config;

// ========================================
// CONFIGURATION GUIDE
// ========================================
/*
┌─────────────────────────────────────────┐
│  SETUP INSTRUCTIONS                     │
└─────────────────────────────────────────┘

1. AUTHORIZED NUMBERS
   ├─ Add phone numbers to ALLOWED_NUMBERS array
   ├─ Format: Just digits, no + or country code
   ├─ Example: '9779800000000' (Nepal)
   ├─ Example: '1234567890' (US)
   └─ Example: '919876543210' (India)

2. BOT CONFIGURATION
   ├─ sessionId: Unique name for this bot session
   ├─ authTimeout: Time to scan QR (default: 120s)
   ├─ headless: true = background, false = visible browser
   └─ multiDevice: Keep true for WhatsApp Web support

3. MESSAGE CUSTOMIZATION
   ├─ Edit messages in MESSAGES object
   ├─ Use variables: {phone}, {category}, {folder}, {filename}, {count}, {size}
   ├─ Keep formatting for professional appearance
   └─ Test messages after changes

4. FOLDER STRUCTURE
   The system creates this structure automatically:
   
   downloads/
   ├── Photo Category 1/
   │   ├── 1234567890/
   │   │   ├── 1715234567890.jpg (Photo #1)
   │   │   ├── 1715234567891.jpg (Photo #2)
   │   │   └── 1715234567892.jpg (Photo #3)
   │   └── 9876543210/
   ├── Photo Category 2/
   │   └── 1234567890/
   ├── Photo Category 3/
   │   └── 9876543210/
   └── Photo Category 4/
       └── 5551234567/

5. USER WORKFLOW EXAMPLE
   Step 1: User sends → 1234567890
   Step 2: Bot shows categories with numbers
   Step 3: User sends → 2 (selects Photo Category 2)
   Step 4: Bot confirms folder creation
   Step 5: User sends photos
   Result: Photos saved as:
           downloads/Photo Category 2/1234567890/[timestamp].jpg
           Each photo numbered: "Photo #1 saved", "Photo #2 saved", etc.

6. SUPPORTED FORMATS
   ├─ Images: JPG, PNG, HEIC, HEIF, GIF, WEBP (all convert to JPG)
   ├─ Videos: MP4, AVI, MOV
   └─ Documents: PDF, DOC, DOCX, TXT

7. FEATURES
   ├─ Automatic photo counting per folder
   ├─ HEIC/HEIF to JPG conversion
   ├─ All images standardized to JPG format
   ├─ File size validation (50MB max)
   ├─ Professional status messages
   └─ Existing photo count on folder activation

8. FORCE NEW SESSION
   If you need to re-authenticate:
   ├─ Uncomment: sessionData: "NUKE"
   ├─ Run the bot
   ├─ Scan new QR code
   └─ Re-comment the line

9. PHONE NUMBER FORMATS ACCEPTED
   Users can send numbers in any format:
   ├─ +977-980-98098
   ├─ 97798098098
   ├─ 98098098
   ├─ +1 (555) 123-4567
   └─ The bot automatically cleans and validates

10. TROUBLESHOOTING
    ├─ QR not generating? Check authTimeout value
    ├─ Photos not saving? Check ALLOWED_NUMBERS
    ├─ Conversion failing? Verify heic-convert installed
    └─ Session issues? Use sessionData: "NUKE"

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
For support or questions, refer to the documentation
or contact your system administrator.
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/