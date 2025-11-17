// ========================================
// WHATSAPP PHOTO MANAGER CONFIGURATION
// ========================================
//
// This is a template configuration file.
// Edit this file with your settings before running the app.
//

module.exports = {
	// ========================================
	// AUTHORIZED PHONE NUMBERS ⚙️ REQUIRED
	// ========================================
	// Define which phone numbers are allowed to use this service
	// Format: Number only, without + or country code prefix
	//
	// Examples:
	//   '9779867936480'  (Nepal)
	//   '1234567890'     (USA)
	//   '919876543210'   (India)
	//
	ALLOWED_NUMBERS: [
		// ⬇️ ADD YOUR NUMBERS HERE ⬇️
		// '9779867936480',
		// '1234567890',
		// '919876543210',
	],

	// ========================================
	// BOT CONFIGURATION
	// ========================================
	BOT_CONFIG: {
		sessionId: "photo-manager-session",
		authTimeout: 120,
		qrTimeout: 120,
		multiDevice: true,
		headless: true,
		deleteSessionDataOnLogout: true,
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
		"4x6 Size Photo",
		"A4 Photo Frame",
		"Polaroid Photo",
		"18x24 Banner",
		// Add more categories as needed:
		// 'Custom Category 1',
		// 'Custom Category 2',
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

📝 Example: First send "9779867936480", then send "2"`,

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
   • Example: 9779867936480

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

// ========================================
// QUICK SETUP GUIDE
// ========================================
/*

1️⃣ EDIT ALLOWED_NUMBERS
   ├─ Find the ALLOWED_NUMBERS array (around line 20)
   ├─ Add your authorized phone numbers
   ├─ Format: Just digits (e.g., '9779867936480')
   ├─ You can add multiple numbers
   └─ Example:
      ALLOWED_NUMBERS: [
          '9779867936480',  // Your number
          '1234567890',     // A friend's number
      ],

2️⃣ (OPTIONAL) CUSTOMIZE CATEGORIES
   ├─ Find PHOTO_CATEGORIES (around line 50)
   ├─ Edit the category names to your preference
   └─ Example:
      PHOTO_CATEGORIES: [
          'Personal',
          'Work',
          'Archive',
      ],

3️⃣ (OPTIONAL) CUSTOMIZE MESSAGES
   ├─ Find the MESSAGES object (around line 65)
   ├─ Edit any messages you want to customize
   └─ Keep {variables} intact

4️⃣ SAVE & RUN
   ├─ Save this file (Ctrl+S or Cmd+S)
   ├─ Run the app: double-click run-app.bat (Windows) or ./photo-manager (Mac/Linux)
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
   • You have Node.js installed (for dev mode)
   • Try running again or check console for errors

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
   • Add it to ALLOWED_NUMBERS
   • Save this file
   • Restart the app

More help? See PACKAGING.md or CI-CD.md

*/
