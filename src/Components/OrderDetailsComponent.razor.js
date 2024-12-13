export function discountTooltip(elementId, value) {
    var rangeInput = document.getElementById(elementId);
    if (rangeInput == null) {
        return;
    }
    if (rangeInput._tooltipInstance) {
        if (value == null) {
            return;
        }
        rangeInput._tooltipInstance.dispose();
    }

    rangeInput.setAttribute('title', ((value ?? rangeInput.value) * 100).toFixed(0) + '%');

    var tooltip = new bootstrap.Tooltip(rangeInput);
    rangeInput._tooltipInstance = tooltip;
    if (value != null) {
        tooltip.show();
    }
}