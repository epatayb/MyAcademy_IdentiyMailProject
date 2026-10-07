document.addEventListener("DOMContentLoaded", function () {

    const modalElement = document.getElementById("categoryModal");

    if (!modalElement) {
        return;
    }

    const categoryModal = bootstrap.Modal.getOrCreateInstance(modalElement);

    const createButton = document.getElementById("createCategoryButton");

    const idInput = modalElement.querySelector("#Id");
    const nameInput = modalElement.querySelector("#Name");
    const descriptionInput = modalElement.querySelector("#Description");
    const colorInput = modalElement.querySelector("#ColorHex");

    const modalTitle = document.getElementById("categoryModalTitle");

    const customColorInput = document.getElementById("categoryCustomColor");

    const colorPreview = document.getElementById("categorySelectedColorPreview");

    const colorText = document.getElementById("categorySelectedColorText");

    const colorButtons = document.querySelectorAll(".category-color-option");


    function setColor(color) {
        const normalizedColor = color.toUpperCase();
        colorInput.value = normalizedColor;
        colorPreview.style.backgroundColor = normalizedColor;
        colorText.textContent = normalizedColor;
        customColorInput.value = normalizedColor;
        colorButtons.forEach(button => {
            button.classList.toggle("is-selected", button.dataset.color.toUpperCase() === normalizedColor);
        });
    }

    function resetForm() {
        idInput.value = "";
        nameInput.value = "";
        descriptionInput.value = "";
        modalTitle.textContent = "Yeni Kategori Oluştur";
        setColor("#1D4ED8");
    }


    createButton?.addEventListener("click", function () {
        resetForm();
        categoryModal.show();
        setTimeout(() => nameInput.focus(), 150);
    });


    document
        .querySelectorAll(".edit-category-button")
        .forEach(button => {

            button.addEventListener("click", function () {

                idInput.value = button.dataset.id;

                nameInput.value = button.dataset.name ?? "";

                descriptionInput.value = button.dataset.description ?? "";

                modalTitle.textContent = "Kategoriyi Düzenle";

                setColor(button.dataset.color);

                categoryModal.show();
            });

        });


    colorButtons.forEach(button => {
        button.addEventListener("click", function () {
            setColor(button.dataset.color);
        });
    });


    customColorInput?.addEventListener("input", function () {
        setColor(customColorInput.value);
    });

    const deleteModalElement = document.getElementById("deleteCategoryModal");
    const deleteModal = bootstrap.Modal.getOrCreateInstance(deleteModalElement);
    const deleteCategoryId = document.getElementById("deleteCategoryId");
    const deleteCategoryText = document.getElementById("deleteCategoryText");

    document
        .querySelectorAll(".delete-category-button")
        .forEach(button => {
            button.addEventListener("click", function () {

                deleteCategoryId.value = button.dataset.id;

                deleteCategoryText.textContent = `"${button.dataset.name}" kategorisini silmek istediğinize emin misiniz?`;

                deleteModal.show();
            });
        });


    if (modalElement.dataset.open === "true") {
        setColor(colorInput.value || "#1D4ED8");
        categoryModal.show();
    }
    else {
        setColor(colorInput.value || "#1D4ED8");
    }
});