$(function () {
    getProducts()

    $('#priceUp, #priceDown').on('click', function () {
        var filter = $('#maxPrice').html();
        var val = Number($(this).data('val'));
        if (val > 0) {
            filter = filter == "none" ? val : Number(filter) + val;
        } else if (filter != "none") {
            filter = Number(filter) + val <= 0 ? "none" : Number(filter) + val;
        }
        $('#maxPrice').html(filter);
        getProducts();
    });

    $('#priceFilter').on('click', function () {
        $('#priceBar').toggle();
    });

    // delegated event listener
    $('#product_table').on('click', 'tr', function () {
        // make sure a customer is logged in
        if ($('#User').data('customer').toLowerCase() == "true") {
            $('#ProductId').html($(this).data('id'));
            $('#ProductName').html($(this).data('name'));
            $('#UnitPrice').html($(this).data('price').toFixed(2));
            // calculate and display total in modal
            $('#Quantity').change();
            $('#cartModal').modal();
        } else {
            toast("Access Denied", "You must be signed in as a customer to access the cart.");
        }
    });

    $('#addToCart').on('click', function () {
        $('#cartModal').modal('hide');
        // AJAX to update database
        $.ajax({
            headers: { "Content-Type": "application/json" },
            url: "../../api/addtocart",
            type: 'post',
            data: JSON.stringify({
                "id": $('#ProductId').html(),
                "email": $('#User').data('email'),
                "qty": $('#Quantity').val()
            }),
            success: function (response, textStatus, jqXhr) {
                // success
                toast("Product Added", response.product.productName + " successfully added to cart.");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                // log the error to the console
                console.log("The following error occured: " + jqXHR.status, errorThrown);
                toast("Error", "Please try again later.");
            }
        });
    });


    // update total when cart quantity is changed
    $('#Quantity').change(function () {
        var total = parseInt($(this).val()) * parseFloat($('#UnitPrice').html());
        $('#Total').html(numberWithCommas(total.toFixed(2)));
    });

    function toast(header, message) {
        $('#toast_header').html(header);
        $('#toast_body').html(message);
        $('#cart_toast').toast({ delay: 2500 }).toast('show');
    }


    // function to display commas in number
    function numberWithCommas(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }

    function getProducts() {
        var id = $('#product_table').data('id');
        var filter = $('#maxPrice').html() == "none" ? "" : "/maxprice/" + $('#maxPrice').html();

        $.getJSON({
            url: "../../api/category/" + id + "/product/discontinued/false" + filter,
            success: function (response, textStatus, jqXhr) {
                //console.log(response);
                $('#product_table').html("");
                for (var i = 0; i < response.length; i++) {
                    var row = "<tr data-id=\"" + response[i].productId + "\" data-name=\"" + response[i].productName + "\" data-price=\"" + response[i].unitPrice + "\">"
                        + "<td>" + response[i].productName + "</td>"
                        + "<td class=\"text-right\">$" + response[i].unitPrice.toFixed(2) + "</td>"
                        + "<td class=\"text-right\">" + response[i].unitsInStock + "</td>"
                        + "</tr>";
                    $('#product_table').append(row);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                // log the error to the console
                console.log("The following error occured: " + textStatus, errorThrown);
            }
        });
    }
});
