document.addEventListener("DOMContentLoaded", function () {
    document
        .querySelectorAll(".category-compose-selector")
        .forEach(selector => {

            const valueInput = selector.querySelector(".category-compose-value");

            const selectedDot = selector.querySelector(".category-compose-dot");

            const selectedName = selector.querySelector(".category-compose-name");

            const options = selector.querySelectorAll(".category-compose-option");

            options.forEach(option => {

                option.addEventListener(
                    "click",
                    function () {

                        const categoryId = option.dataset.categoryId ?? "";

                        const categoryName = option.dataset.categoryName ?? "Kategori Yok";

                        const categoryColor = option.dataset.categoryColor ?? "#98A2B3";

                        valueInput.value = categoryId;

                        selectedName.textContent = categoryName;

                        selectedDot.style.backgroundColor = categoryColor;

                        options.forEach(x =>
                            x.classList.remove("is-selected")
                        );

                        option.classList.add("is-selected");
                    });
            });
        });
});