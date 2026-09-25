// ========================================
// WHATSAPP PHOTO MANAGER CONFIGURATION
// ========================================
//
// This is a template configuration file.
// Edit this file with your settings before running the app.
//

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
	// AUTHORIZED PHONE NUMBERS ⚙️ REQUIRED
	// ========================================
	// Define which phone numbers are allowed to use this service
	// Format: Number only, without + or country code prefix
	//
	// Examples:
	//   '1234567890'     (USA)
	//   '9779800000000'  (Nepal)
	//   '919876543210'   (India)
	//
	ALLOWED_NUMBERS: [
		// ⬇️ ADD YOUR NUMBERS HERE ⬇️
		// '1234567890',
		// '9779800000000',
		// '919876543210',
	],

	// ========================================
	// BOT CONFIGURATION
	// ========================================
	BOT_CONFIG: {
		sessionId: "photo-manager-session",
		useChrome: true,
		authTimeout: 0,
		qrTimeout: 0,
		protocolTimeout: 0,
		multiDevice: true,
		safeMode: true,
		headless: true,
		deleteSessionDataOnLogout: false,
		killClientOnLogout: false,
		eventMode: false,
		waitForRipeSession: true,
		restartOnCrash: true,
		inDocker: true,
		sessionDataPath: '.',
		customUserAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36',
	},

	// ========================================
	// FOLDER & FILE SETTINGS
	// ========================================
	FOLDER_SETTINGS: {
		downloadsDir: "downloads",
		maxFileSize: 50 * 1024 * 1024,
	},

	// ========================================
	// PHOTO CATEGORIES
	// ========================================
	// Customize these categories based on your needs
	PHOTO_CATEGORIES: [
		"Photo Category 1",
		"Photo Category 2",
		"Photo Category 3",
		"Photo Category 4",
		// Add more categories as needed:
		// 'Photo Category 5',
		// 'Photo Category 6',
	],

	// ========================================
	// BOT MESSAGES
	// ========================================
	// Customize all bot responses here
	// Variables: {phone}, {category}, {folder}, {filename}, {count}, {size}
	MESSAGES: {
		unauthorized: `🚫 **Access Denied**

You are not authorized to use this service.
Please contact the administrator for access.`,

		selectCategory: `📂 **Select Photo Category**
━━━━━━━━━━━━━━━━━━━━━━━━
📱 Phone Number: *{phone}*

Please choose the photo category:
{options}

💡 Reply with the number (1-N) to continue.`,

		phoneNumberFirst: `⚠️ **Incorrect Order**
━━━━━━━━━━━━━━━━━━━━━━━━

Please follow the correct workflow:

**Step 1:** Send phone number (7-15 digits)
**Step 2:** Select category (1-N)

📝 Example: First send "1234567890", then send "2"`,

		folderCreated: `✅ **Folder Activated Successfully**
━━━━━━━━━━━━━━━━━━━━━━━━
📱 Phone: *{phone}*
📂 Category: *{category}*{count}

🎯 Ready to receive photos! Send images now.`,

		photoSaved: `✅ Photo saved successfully in folder "{folder}" as {filename}
Total: {count}`,

		videoSaved: `✅ Video saved successfully in folder "{folder}" as {filename}`,

		documentSaved: `✅ Document saved successfully in folder "{folder}" as {filename}`,

		noFolderSet: `❌ **No Active Folder**
━━━━━━━━━━━━━━━━━━━━━━━━

Please set up a folder first:

**Step 1:** Send phone number (7-15 digits)
**Step 2:** Select category (1-N)
**Step 3:** Send photos

💡 Type "help" for detailed instructions.`,

		errorSaving: `❌ **Save Failed**
━━━━━━━━━━━━━━━━━━━━━━━━

Unable to save the file. Please try again.

If the problem persists:
• Check file size (max 50MB)
• Ensure file is not corrupted
• Contact administrator if issue continues`,

		conversionError: `❌ **Image Conversion Failed**
━━━━━━━━━━━━━━━━━━━━━━━━

Unable to convert HEIC/HEIF image format.

Please try:
• Converting to JPG on your device
• Sending as a different format
• Sending as a document instead`,

		instructions: `📖 **Photo Manager - User Guide**
━━━━━━━━━━━━━━━━━━━━━━━━

**🔄 Workflow:**

**Step 1:** Send Phone Number
   • Format: 7-15 digits
   • Example: 1234567890

**Step 2:** Select Category
{options}

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
	},
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
// QUICK SETUP GUIDE
// ========================================
/*

1️⃣ EDIT ALLOWED_NUMBERS
   ├─ Find the ALLOWED_NUMBERS array (around line 40)
   ├─ Add your authorized phone numbers
   ├─ Format: Just digits (e.g., '1234567890')
   ├─ You can add multiple numbers
   └─ Example:
      ALLOWED_NUMBERS: [
          '1234567890',     // Primary number
          '9876543210',     // Secondary number
      ],

2️⃣ (OPTIONAL) CUSTOMIZE CATEGORIES
   ├─ Find PHOTO_CATEGORIES
   ├─ Edit the category names to your preference
   └─ Example:
      PHOTO_CATEGORIES: [
          'Personal',
          'Work',
          'Archive',
      ],

3️⃣ (OPTIONAL) CUSTOMIZE MESSAGES
   ├─ Find the MESSAGES object
   ├─ Edit any messages you want to customize
   └─ Keep {variables} intact

4️⃣ SAVE & RUN
   ├─ Save this file as config.js
   ├─ Run the app: double-click whatsapp-photo-manager.exe or npm start
   └─ Scan the QR code on your phone

5️⃣ DONE! 🎉
   ├─ Start sending phone numbers and photos
   ├─ Files will be organized automatically
   └─ Check the 'downloads' folder

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
HELP & TROUBLESHOOTING
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Q: The app won't start
A: Make sure:
   • This file is saved (no errors)
   • You have Node.js installed (for dev mode) or use the Windows installer
   • Try running again or check console logs

Q: QR code won't scan
A: Try:
   • Ensuring WhatsApp is installed on your phone
   • Checking your internet connection
   • Waiting a bit longer for the QR to generate

Q: Photos aren't saving
A: Check:
   • Your number is in ALLOWED_NUMBERS
   • You've selected a category
   • Disk space is available
   • File size is under 50MB

Q: I forgot to add my number
A: Just:
   • Open Settings in the app or add it to ALLOWED_NUMBERS
   • Save this file
   • Restart the app

*/
