document.addEventListener("DOMContentLoaded", function () {

    document
        .querySelectorAll(".category-selector")
        .forEach(selector => {

            const select = selector.querySelector(".category-selector-select");
            const dot = selector.querySelector(".category-selector-dot");

            if (!select || !dot) {
                return;
            }


            function updateColor() {
                const option = select.options[select.selectedIndex];
                const color = option?.dataset?.color;
                dot.style.backgroundColor = color || "#c2c8d2";
            }

            select.addEventListener("change", updateColor);
            updateColor();
        });
});