# WhatsApp Photo Manager

A professional WhatsApp bot for managing and organizing photos by phone numbers and categories.

## Features

- **📱 WhatsApp Integration**: Connects to WhatsApp Web for automated media management
- **🔐 Authorization System**: Only pre-approved phone numbers can use the service
- **📁 Automatic Organization**: Creates folders for each phone number and saves media there
- **📂 Category System**: Organize media into customizable categories
- **📸 Multi-Media Support**: Handles images, videos, and documents
- **📊 Detailed Logging**: Tracks all activities with timestamps and file sizes
- **🔄 Session Management**: Easy session reset and QR code authentication

## Quick Start

### Prerequisites

- Node.js 14+ 
- WhatsApp account

### Installation

1. Clone the repository:
```bash
git clone <your-repo-url>
cd photo-manager
```

2. Install dependencies:
```bash
npm install
```

3. Configure the bot:
   - Edit `config.js` to add your authorized phone numbers
   - Update other settings as needed

4. Start the bot:
```bash
npm start
# or
./start.sh
# or on Windows
start.bat
```

### Configuration

Edit `config.js` to customize:

- **Authorized Numbers**: Add phone numbers that can use the service
- **Photo Categories**: Define custom categories for organizing media
- **Bot Settings**: Session management, timeouts, etc.
- **Messages**: Customize all bot responses
- **Folder Structure**: Configure download directory

### Usage

1. **Send Phone Number**: Send a 7-15 digit phone number
2. **Select Category**: Choose from available categories (configurable)
3. **Upload Photos**: Send images, videos, or documents
4. **Automatic Organization**: Files are saved in organized folder structure

### Folder Structure

```
downloads/
├── [Category 1]/
│   └── [phone-number]/
├── [Category 2]/
│   └── [phone-number]/
├── [Category 3]/
│   └── [phone-number]/
└── [Category N]/
    └── [phone-number]/
```

### Supported Formats

- **Images**: JPG, PNG, HEIC, HEIF, GIF, WEBP (converted to JPG)
- **Videos**: MP4, AVI, MOV
- **Documents**: PDF, DOC, DOCX, TXT

## Configuration Guide

### Adding Authorized Numbers

```javascript
ALLOWED_NUMBERS: [
    '9779867936480',  // Nepal
    '1234567890',     // US
    '919876543210',   // India
],
```

### Configuring Photo Categories

```javascript
PHOTO_CATEGORIES: [
    'Category 1',
    'Category 2', 
    'Category 3',
    'Category 4'
],
```

### Customizing Messages

Edit the `MESSAGES` object in `config.js` to customize all bot responses.

### Bot Settings

- `sessionId`: Unique session identifier
- `authTimeout`: QR scan timeout (default: 120s)
- `headless`: Run browser in background
- `multiDevice`: Enable WhatsApp multi-device support

## License

MIT License - see LICENSE file for details.

## Support

For issues or questions, please contact the administrator.