<?php
/**
 * Template Name: Mankan Top Page
 *
 * Cocoon 固定ページ設定の推奨:
 * - サイドバー: なし
 * - タイトル: 非表示
 *
 * @package MankanCocoonChild
 */

get_header();
?>

<main id="primary" class="site-main mankan-top">
	<section class="mankan-hero">
		<div class="mankan-hero__inner">
			<h1 class="mankan-hero__heading">管理会社の「言い値」を疑え。</h1>
			<p class="mankan-hero__subheading">マンション管理士 × 宅建 × 管理業務主任者</p>
			<div class="mankan-hero__cta-grid">
				<a href="<?php echo esc_url( home_url( '/contact/?service=estimate' ) ); ?>" class="mankan-cta mankan-cta--primary">
					管理費の見積もり相談
				</a>
				<a href="<?php echo esc_url( home_url( '/contact/?service=consulting' ) ); ?>" class="mankan-cta mankan-cta--secondary">
					管理会社選びの相談
				</a>
			</div>
		</div>
	</section>
</main>

<?php
get_footer();
