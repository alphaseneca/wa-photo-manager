const test = require('node:test');
const assert = require('node:assert/strict');
const path = require('node:path');
const fs = require('node:fs');

test('Configuration Module Suite', async (t) => {
    const configPath = path.join(__dirname, '..', 'config.js');
    const templatePath = path.join(__dirname, '..', 'config.template.js');

    await t.test('config.js and config.template.js exist', () => {
        assert.ok(fs.existsSync(configPath), 'config.js must exist');
        assert.ok(fs.existsSync(templatePath), 'config.template.js must exist');
    });

    await t.test('Default config loads with production-grade generic categories', () => {
        const config = require(configPath);
        
        assert.ok(Array.isArray(config.ALLOWED_NUMBERS), 'ALLOWED_NUMBERS must be an array');
        assert.ok(Array.isArray(config.PHOTO_CATEGORIES), 'PHOTO_CATEGORIES must be an array');
        assert.ok(config.PHOTO_CATEGORIES.length > 0, 'PHOTO_CATEGORIES must not be empty');
        
        // Ensure no hardcoded specific merchant categories remain
        for (const cat of config.PHOTO_CATEGORIES) {
            assert.match(cat, /^Photo Category \d+$/, `Category "${cat}" should follow generic format "Photo Category N"`);
        }
    });

    await t.test('Bot configuration object has required operational parameters', () => {
        const config = require(configPath);
        
        assert.ok(config.BOT_CONFIG, 'BOT_CONFIG object must exist');
        assert.equal(typeof config.BOT_CONFIG.sessionId, 'string', 'sessionId must be a string');
        assert.ok(config.BOT_CONFIG.sessionId.length > 0, 'sessionId cannot be empty');
        assert.equal(typeof config.BOT_CONFIG.headless, 'boolean', 'headless flag must be boolean');
        assert.equal(typeof config.BOT_CONFIG.multiDevice, 'boolean', 'multiDevice flag must be boolean');
    });

    await t.test('Folder settings define valid downloads path and size limits', () => {
        const config = require(configPath);
        
        assert.ok(config.FOLDER_SETTINGS, 'FOLDER_SETTINGS must exist');
        assert.equal(typeof config.FOLDER_SETTINGS.downloadsDir, 'string');
        assert.ok(config.FOLDER_SETTINGS.downloadsDir.length > 0);
        assert.ok(config.FOLDER_SETTINGS.maxFileSize > 0, 'maxFileSize must be greater than 0');
    });

    await t.test('Messages dictionary includes all interactive bot prompts', () => {
        const config = require(configPath);
        
        assert.ok(config.MESSAGES, 'MESSAGES dictionary must exist');
        assert.ok(config.MESSAGES.unauthorized, 'unauthorized message must exist');
        assert.ok(config.MESSAGES.selectCategory, 'selectCategory message must exist');
        assert.ok(config.MESSAGES.folderCreated, 'folderCreated message must exist');
        assert.ok(config.MESSAGES.photoSaved, 'photoSaved message must exist');
    });
});
