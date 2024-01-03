(function ($) {
    //    "use strict";


    /*  Data Table
    -------------*/

    $('#bootstrap-data-table').DataTable({
        lengthMenu: [[10, 20, 50, -1], [10, 20, 50, "All"]],
    });

	$('#bootstrap-data-table-export').DataTable({
		order: [],
        lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
		buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
		"columnDefs": [
			{
				"targets": [0, 1, 2, 3, 4],
				"orderable": false,
			},
			{
				"targets": 4,
				"searchable": false
			}
		]
	});

	$('#bootstrap-data-table-export1').DataTable({
		order: [],
		lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
		buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
		"columnDefs": [
			{
				"targets": 4,
				"orderable": false,
			},
			{
				"targets": 4,
				"searchable": false
			}
		]
	});

	$('#bootstrap-data-table-export2').DataTable({
		order: [],
		lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
		buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
		"columnDefs": [
			{
				"targets": 3,
				"orderable": false,
			}
		]
	});

	$('#bootstrap-data-table-export3').DataTable({
		order: [],
		lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
		buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
		"columnDefs": [
			{
				"targets": [0, 1, 2],
				"orderable": false,
			},
			{
				"targets": 3,
				"searchable": false
			}
		]
	});

	$('#row-select').DataTable( {
        initComplete: function () {
				this.api().columns().every( function () {
					var column = this;
					var select = $('<select class="form-control"><option value=""></option></select>')
						.appendTo( $(column.footer()).empty() )
						.on( 'change', function () {
							var val = $.fn.dataTable.util.escapeRegex(
								$(this).val()
							);

							column
								.search( val ? '^'+val+'$' : '', true, false )
								.draw();
						} );

					column.data().unique().sort().each( function ( d, j ) {
						select.append( '<option value="'+d+'">'+d+'</option>' )
					} );
				} );
			}
		} );

})(jQuery);
