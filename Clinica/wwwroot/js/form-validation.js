document.addEventListener("DOMContentLoaded", function () {
    // Máscara para Fecha de Nacimiento
    const fechaNacimientoInput = document.getElementById("FechaNacimiento");
    fechaNacimientoInput.addEventListener("input", function () {
        const value = fechaNacimientoInput.value.replace(/[^0-9/]/g, "").slice(0, 10);
        if (value.length === 2 || value.length === 5) {
            fechaNacimientoInput.value = value + "/";
        } else {
            fechaNacimientoInput.value = value;
        }
    });

    // Validación de Email
    const emailInput = document.getElementById("Email");
    emailInput.addEventListener("blur", function () {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(emailInput.value)) {
            alert("Por favor, ingresa un correo electrónico válido.");
            emailInput.focus();
        }
    });

    // Validación de Teléfono
    const telefonoInput = document.getElementById("Telefono");
    telefonoInput.addEventListener("blur", function () {
        const telefonoRegex = /^\d{8}$/; // Acepta exactamente 8 dígitos
        if (!telefonoRegex.test(telefonoInput.value)) {
            alert("Por favor, ingresa un número de teléfono válido (8 dígitos).");
            telefonoInput.focus();
        }
    });

});
