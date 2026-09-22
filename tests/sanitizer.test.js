const test = require('node:test');
const assert = require('node:assert/strict');
const path = require('node:path');

// Helper functions that mirror the sanitization logic in the bot
function cleanPhoneNumber(input) {
    if (!input || typeof input !== 'string') return '';
    return input.replace(/[^0-9]/g, '');
}

function isValidPhoneNumber(phone) {
    const cleaned = cleanPhoneNumber(phone);
    return cleaned.length >= 7 && cleaned.length <= 15;
}

function sanitizeFolderName(name) {
    if (!name || typeof name !== 'string') return 'Unknown';
    // Remove characters forbidden on Windows/POSIX filesystems: \ / : * ? " < > |
    return name.replace(/[\\/:*?"<>|]/g, '').trim();
}

function isAllowedExtension(filename, allowedExtensions = ['.jpg', '.jpeg', '.png', '.heic', '.heif', '.mp4', '.avi', '.mov', '.pdf', '.docx']) {
    const ext = path.extname(filename).toLowerCase();
    return allowedExtensions.includes(ext);
}

test('Sanitizer & Validation Utility Suite', async (t) => {
    await t.test('cleanPhoneNumber strips formatting and non-digits', () => {
        assert.equal(cleanPhoneNumber('+1 (555) 123-4567'), '15551234567');
        assert.equal(cleanPhoneNumber('977-9800-000000'), '9779800000000');
        assert.equal(cleanPhoneNumber(' 1234567890 \n'), '1234567890');
        assert.equal(cleanPhoneNumber(null), '');
    });

    await t.test('isValidPhoneNumber enforces 7 to 15 digits', () => {
        assert.equal(isValidPhoneNumber('1234567'), true, '7 digits should be valid');
        assert.equal(isValidPhoneNumber('123456789012345'), true, '15 digits should be valid');
        assert.equal(isValidPhoneNumber('123456'), false, '6 digits should be invalid');
        assert.equal(isValidPhoneNumber('1234567890123456'), false, '16 digits should be invalid');
        assert.equal(isValidPhoneNumber('not-a-number'), false, 'Text should be invalid');
    });

    await t.test('sanitizeFolderName removes prohibited filesystem characters', () => {
        assert.equal(sanitizeFolderName('Category: 1 / 2'), 'Category 1  2');
        assert.equal(sanitizeFolderName('Photo*Archive?'), 'PhotoArchive');
        assert.equal(sanitizeFolderName('Legal "Bills" <2026> | Main'), 'Legal Bills 2026  Main');
        assert.equal(sanitizeFolderName(''), 'Unknown');
    });

    await t.test('isAllowedExtension identifies accepted media types', () => {
        assert.equal(isAllowedExtension('photo.jpg'), true);
        assert.equal(isAllowedExtension('image.PNG'), true);
        assert.equal(isAllowedExtension('raw.HEIC'), true);
        assert.equal(isAllowedExtension('video.mp4'), true);
        assert.equal(isAllowedExtension('script.exe'), false);
        assert.equal(isAllowedExtension('malware.bat'), false);
    });
});
