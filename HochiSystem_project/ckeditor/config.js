/**
 * @license Copyright (c) 2003-2019, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';
	
    config.toolbar = [
		//{ name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
		{ name: 'links', items: ['Link', 'Unlink'] },
        { name: 'insert', items: ['Image', 'Table', 'HorizontalRule', 'SpecialChar', 'Smiley'] },
        { name: 'document', items: ['Source', '-', 'Print'] },
        '/',
        { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', '-', 'RemoveFormat'] },
        { name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', 'Blockquote', '-'] },
        { name: 'styles', items: ['Styles', 'Format', 'FontSize'] },
        { name: 'colors', items: ['TextColor'] }
	];
	config.enterMode = CKEDITOR.ENTER_BR;

    

};
