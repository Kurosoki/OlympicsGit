function toLowerCaseInput(event) {
    event.target.value = event.target.value.toLowerCase();
}

function scrollToElement(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
}
