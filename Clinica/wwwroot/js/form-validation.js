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

    document.addEventListener("DOMContentLoaded", function () {
        // Máscara para Teléfono
        const telefonoInput = document.getElementById("Telefono");
        telefonoInput.addEventListener("input", function () {
            let value = telefonoInput.value.replace(/\D/g, ""); // Elimina todo lo que no sea dígito
            value = value.slice(0, 10); // Limita a 10 caracteres

            let formattedValue = value;
            if (value.length >= 1) formattedValue = `(${value.slice(0, 3)}`;
            if (value.length >= 4) formattedValue += `) ${value.slice(3, 6)}`;
            if (value.length >= 7) formattedValue += `-${value.slice(6, 10)}`;

            telefonoInput.value = formattedValue;
        });
    });


});
