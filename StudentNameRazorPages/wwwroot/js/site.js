// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Hiệu ứng đổi màu navbar khi cuộn
window.addEventListener('scroll', function() {
    var navbar = document.querySelector('.navbar');
    if(window.scrollY > 30) {
        navbar.classList.add('scrolled');
    } else {
        navbar.classList.remove('scrolled');
    }
});
