// ========================================
// WHATSAPP PHOTO MANAGER CONFIGURATION
// ========================================
// Professional photo management system for WhatsApp
// Edit this file to customize your bot settings

module.exports = {
    // ========================================
    // AUTHORIZED PHONE NUMBERS
    // ========================================
    // Define which phone numbers are allowed to use this service
    // Format: Number only, without + or country code prefix
    // Examples: '9779867936480', '1234567890', '919876543210'
    
    ALLOWED_NUMBERS: [
        '9779867936480',  // Primary authorized number
        // Add additional authorized numbers below:
        // '1234567890',
        // '9876543210',
    ],

    // ========================================
    // BOT CONFIGURATION
    // ========================================
    // Core WhatsApp bot settings and session management
    
    BOT_CONFIG: {
        sessionId: 'photo-manager-session',      // Unique session identifier
        authTimeout: 120,                        // QR code scan timeout (seconds)
        qrTimeout: 120,                          // QR code generation timeout (seconds)
        multiDevice: true,                       // Enable WhatsApp multi-device support
        headless: true,                          // Run browser in background (set false to debug or re-scan QR)
        deleteSessionDataOnLogout: true,         // Auto-cleanup session data on logout
        waitForRipeSession: true,                // Wait for session to be fully ready before injection
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
        '4x6 Size Photo',
        'A4 Photo Frame', 
        'Polaroid Photo',
        '18x24 Banner'
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

📝 Example: First send "9779867936480", then send "2"`,

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
   • Example: 9779867936480

**Step 2:** Select Category
   1️⃣ 4x6 Size Photo
   2️⃣ A4 Photo Frame
   3️⃣ Polaroid Photo
   4️⃣ 18x24 Banner

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
   ├─ Example: '9779867936480' (Nepal)
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
   ├── 4x6 Size Photo/
   │   ├── 9779867936480/
   │   │   ├── 1234567890.jpg (Photo #1)
   │   │   ├── 1234567891.jpg (Photo #2)
   │   │   └── 1234567892.jpg (Photo #3)
   │   └── 1234567890/
   ├── A4 Photo Frame/
   │   └── 9779867936480/
   ├── Polaroid Photo/
   │   └── 9876543210/
   └── 18x24 Banner/
       └── 5551234567/

5. USER WORKFLOW EXAMPLE
   Step 1: User sends → 9779867936480
   Step 2: Bot shows categories with numbers
   Step 3: User sends → 2 (selects A4 Photo Frame)
   Step 4: Bot confirms folder creation
   Step 5: User sends photos
   Result: Photos saved as:
           downloads/A4 Photo Frame/9779867936480/[timestamp].jpg
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