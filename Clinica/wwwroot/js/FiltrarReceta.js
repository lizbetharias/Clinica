document.addEventListener("DOMContentLoaded", function () {
    // Escuchar eventos de input en el campo de búsqueda
    document.getElementById("buscarMedicamento").addEventListener("input", function () {
        const term = this.value;
        const selectMedicamento = document.getElementById("selectMedicamento");

        if (term.length > 0) {
            fetch(`/Receta/FiltrarMedicamentos?term=${encodeURIComponent(term)}`)
                .then(response => {
                    if (!response.ok) throw new Error("Error al obtener medicamentos");
                    return response.json();
                })
                .then(data => {
                    // Limpiar las opciones actuales
                    selectMedicamento.innerHTML = '<option value="">Seleccione un medicamento</option>';

                    // Llenar el select con los resultados
                    data.forEach(medicamento => {
                        const option = document.createElement("option");
                        option.value = medicamento.id;
                        option.textContent = `${medicamento.nombre} - $${medicamento.precio}`;
                        selectMedicamento.appendChild(option);
                    });
                })
                .catch(error => console.error("Error al filtrar medicamentos:", error));
        } else {
            // Si no hay texto, vaciar el select
            selectMedicamento.innerHTML = '<option value="">Seleccione un medicamento</option>';
        }
    });
});
