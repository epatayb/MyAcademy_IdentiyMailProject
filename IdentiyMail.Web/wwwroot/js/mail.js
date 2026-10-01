document.addEventListener("DOMContentLoaded", function () {

    // #region Mobil sidebar kontrolü
    const sidebar = document.getElementById("mailSidebar");
    const overlay = document.getElementById("mailSidebarOverlay");

    const openButton = document.getElementById("mailSidebarOpen");
    const closeButton = document.getElementById("mailSidebarClose");

    function openSidebar() {
        sidebar?.classList.add("is-open");
        overlay?.classList.add("is-open");
    }

    function closeSidebar() {
        sidebar?.classList.remove("is-open");
        overlay?.classList.remove("is-open");
    }

    openButton?.addEventListener("click", openSidebar);
    closeButton?.addEventListener("click", closeSidebar);
    overlay?.addEventListener("click", closeSidebar);
    // #endregion
});