document.addEventListener("DOMContentLoaded", function () {

    document
        .querySelectorAll(".category-quick-form")
        .forEach(form => {

            const categoryInput = form.querySelector(".category-quick-value");

            const options = form.querySelectorAll(".category-quick-option");

            options.forEach(option => {
                option.addEventListener("click", function () {

                    const categoryId = option.dataset.categoryId;

                    categoryInput.value = categoryId ?? "";

                    form.submit();
                });
            });
        });
});