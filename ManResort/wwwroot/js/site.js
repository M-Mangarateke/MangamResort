// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

"use strict";
// common idea: https://dribbble.com/shots/20364660-Nibble-Health-Identity-Social-Posting
// carousel animation: https://codepen.io/aija/details/xvXWoK
let CHECKED = false;
document.addEventListener("pointerdown", (e) => {
    CHECKED = !CHECKED;
    document.documentElement.style.setProperty("--light", CHECKED ? 1 : 0);
});


document.addEventListener("DOMContentLoaded", function () {
    const reviewsContainer = document.getElementById('reviewsContainer');
    const prevBtn = document.getElementById('prevBtn');
    const nextBtn = document.getElementById('nextBtn');

    let currentIndex = 0;
    const totalItems = reviewsContainer.children.length;

    function showReview(index) {
        reviewsContainer.style.transform = `translateX(${-index * 100}%)`;
    }

    prevBtn.addEventListener('click', function () {
        if (currentIndex > 0) {
            currentIndex--;
        } else {
            currentIndex = totalItems - 1;
        }
        showReview(currentIndex);
    });

    nextBtn.addEventListener('click', function () {
        if (currentIndex < totalItems - 1) {
            currentIndex++;
        } else {
            currentIndex = 0;
        }
        showReview(currentIndex);
    });
});
