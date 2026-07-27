<?php
/**
 * SEO / LLMO helpers for Mankan Cocoon child theme.
 *
 * @package MankanCocoonChild
 */

/**
 * Output meta tags and JSON-LD for LLMO on Mankan top page.
 */
function mankan_output_seo_llmo_meta() {
	if ( ! is_page_template( 'page-mankan-top.php' ) ) {
		return;
	}

	$title       = '管理会社の言い値を疑え | マンション管理の見直し相談';
	$description = 'マンション管理士・宅建・管理業務主任者が、管理費の見積もりと管理会社選びをサポート。';
	$canonical   = home_url( '/' );

	echo '<meta name="description" content="' . esc_attr( $description ) . '">' . "\n";
	echo '<meta name="robots" content="index,follow,max-image-preview:large,max-snippet:-1">' . "\n";
	echo '<link rel="canonical" href="' . esc_url( $canonical ) . '">' . "\n";
	echo '<meta property="og:type" content="website">' . "\n";
	echo '<meta property="og:locale" content="ja_JP">' . "\n";
	echo '<meta property="og:title" content="' . esc_attr( $title ) . '">' . "\n";
	echo '<meta property="og:description" content="' . esc_attr( $description ) . '">' . "\n";
	echo '<meta property="og:url" content="' . esc_url( $canonical ) . '">' . "\n";
	echo '<meta name="twitter:card" content="summary_large_image">' . "\n";

	$schema = array(
		'@context'    => 'https://schema.org',
		'@type'       => 'ProfessionalService',
		'name'        => 'マンション管理 見直し相談',
		'description' => $description,
		'url'         => $canonical,
		'areaServed'  => 'JP',
		'serviceType' => array( 'マンション管理見直し', '管理費見積もり相談' ),
	);

	echo '<script type="application/ld+json">' . wp_json_encode( $schema, JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES ) . '</script>' . "\n";
}
add_action( 'wp_head', 'mankan_output_seo_llmo_meta', 1 );

/**
 * Add llms.txt reference in head for AI discovery.
 */
function mankan_output_llms_link() {
	if ( ! is_page_template( 'page-mankan-top.php' ) ) {
		return;
	}

	$llms_url = get_stylesheet_directory_uri() . '/llms.txt';
	echo '<link rel="alternate" type="text/plain" href="' . esc_url( $llms_url ) . '" title="LLM summary">' . "\n";
}
add_action( 'wp_head', 'mankan_output_llms_link', 2 );
