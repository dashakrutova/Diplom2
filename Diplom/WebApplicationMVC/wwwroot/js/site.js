document.getElementById('searchInput').addEventListener('input', function () {
    let searchValue = this.value.toLowerCase();
    let rows = document.querySelectorAll('table tbody tr');

    rows.forEach(row => {
        let columns = row.querySelectorAll('td');
        let match = false;
        columns.forEach(column => {
            if (column.textContent.toLowerCase().includes(searchValue)) {
                match = true;
            }
        });

        row.style.display = match ? '' : 'none';
    });
});