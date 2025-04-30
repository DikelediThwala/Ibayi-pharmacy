$(document).ready(function () {
    let faqQuestionContainers = document.querySelectorAll(`[unique-script-id="w-w-dm-id"] .faq-question-container`);

    faqQuestionContainers.forEach(function (faqQuestionContainer) {
        faqQuestionContainer.onclick = function () {
            this.parentElement.classList.toggle("active");
        };
    });
});
