
var connection = new signalR.HubConnectionBuilder().withUrl("/hub").build();


connection.on("ReloadPage", function () {
    location.reload();
});

connection.on("ReloadProduct", (data) => {
    var tr = '';
    $.each(data, (k, v) => {
        tr +=
            `
            <tr>
                <td><a>#${v.productId}</a></td>
                <td>${v.productName}</td>
                <td>${v.unitPrice}</td>
                <td>${v.unitsOnOrder}</td>
                <td>${v.unitsInStock}</td>
                <td>${v.unitsInStock}</td>
                <td>${v.discontinued}</td>
                <td>
                    <a href="EditProduct?id=${v.productId}">Edit | Delete</a>
                </td>
            </tr>
            `
    })
    $("#tableBody").html(tr);
});


connection.start().then().catch(function (error) { return console.log(error.toString()) });
