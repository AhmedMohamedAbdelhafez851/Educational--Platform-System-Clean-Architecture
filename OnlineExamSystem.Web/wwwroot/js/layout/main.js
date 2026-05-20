// Main Layout JavaScript
(function () {
    'use strict';

    // Initialize all toasts
    function initToasts() {
        var toastElList = [].slice.call(document.querySelectorAll('.toast'));
        toastElList.forEach(function (toastEl) {
            new bootstrap.Toast(toastEl, { autohide: true, delay: 5000 }).show();
        });
    }

    // Scroll to top functionality
    function initScrollToTop() {
        var scrollBtn = document.getElementById('scrollToTop');
        if (scrollBtn) {
            window.addEventListener('scroll', function () {
                scrollBtn.style.display = window.pageYOffset > 300 ? 'flex' : 'none';
            });
            scrollBtn.addEventListener('click', function () {
                window.scrollTo({ top: 0, behavior: 'smooth' });
            });
        }
    }

    // Adjust mobile layout
    function adjustMobileLayout() {
        if (window.innerWidth <= 768) {
            document.body.style.minHeight = window.innerHeight + 'px';
        }
    }

    // Initialize all components
    function init() {
        initToasts();
        initScrollToTop();
        adjustMobileLayout();
        window.addEventListener('resize', adjustMobileLayout);
        window.addEventListener('orientationchange', adjustMobileLayout);
    }

    // Start when DOM is ready
    document.addEventListener('DOMContentLoaded', init);
})();