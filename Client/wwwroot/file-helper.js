window.triggerFileInputClick = (elementId) => {
    const inputElement = document.getElementById(elementId);
    if (inputElement) {
        inputElement.accept = ".csv"; // Restrict to .csv files
        inputElement.click();
    }
};