// Scroll to top functionality
(function () {
    'use strict';

    const scrollBtn = document.getElementById('scrollToTop');

    if (scrollBtn) {
        window.addEventListener('scroll', function () {
            if (window.scrollY > 300) {
                scrollBtn.classList.add('show');
            } else {
                scrollBtn.classList.remove('show');
            }
        });

        scrollBtn.addEventListener('click', function () {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    }

    // Initialize toasts
    const toastElements = document.querySelectorAll('.toast');
    toastElements.forEach(function (toastEl) {
        new bootstrap.Toast(toastEl, { autohide: true, delay: 5000 }).show();
    });
})();