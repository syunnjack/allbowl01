<?php
/**
 * Mankan Cocoon Child theme functions.
 *
 * @package MankanCocoonChild
 */

require_once get_stylesheet_directory() . '/inc/seo-llmo.php';

/**
 * Enqueue parent and child theme stylesheets.
 */
function mankan_cocoon_child_enqueue_styles() {
	wp_enqueue_style(
		'mankan-cocoon-parent-style',
		get_template_directory_uri() . '/style.css',
		array(),
		null
	);

	$child_css_path = get_stylesheet_directory() . '/style.css';
	wp_enqueue_style(
		'mankan-cocoon-child-style',
		get_stylesheet_uri(),
		array( 'mankan-cocoon-parent-style' ),
		file_exists( $child_css_path ) ? (string) filemtime( $child_css_path ) : '1.0.0'
	);
}
add_action( 'wp_enqueue_scripts', 'mankan_cocoon_child_enqueue_styles' );

/**
 * Enqueue Mankan Top Page stylesheet when the custom template is active.
 */
function mankan_cocoon_child_enqueue_top_page_styles() {
	if ( ! is_page_template( 'page-mankan-top.php' ) ) {
		return;
	}

	$css_path = get_stylesheet_directory() . '/assets/css/mankan-style.css';
	if ( ! file_exists( $css_path ) ) {
		return;
	}

	wp_enqueue_style(
		'mankan-style',
		get_stylesheet_directory_uri() . '/assets/css/mankan-style.css',
		array( 'mankan-cocoon-child-style' ),
		(string) filemtime( $css_path )
	);
}
add_action( 'wp_enqueue_scripts', 'mankan_cocoon_child_enqueue_top_page_styles' );
