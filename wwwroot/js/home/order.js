$(document).ready(function () {
  var email = $("#email").text();
  var cus_id;
  $.ajax({
    url: "/api/Customer/getcustomer",
    type: "GET",
    dataType: "json",
    data: { emailAddress: email },
    success: function (data) {
      cus_id = data[0].id;
      $.ajax({
        url: "/api/Order/getorders",
        method: "GET",
        dataType: "json",
        data: { cus_id: cus_id },
        success: function (data) {
          if (!data || Object.keys(data).length === 0) {
            $("#checkout").hide();
          }
          var tbody = $("#OrderTable tbody");
          tbody.empty();
          data.forEach(function (order, index) {
            var row =
              "<tr>" +
              "<td>" +
              order.prodName +
              "</td>" +
              '<td><img class="img-fluid" src="' +
              order.prodImgUrl +
              '" width="100" height="100"/></td>' +
              "<td>₱" +
              '<span class="price">' +
              order.orderItemPrice.toFixed(2) +
              "</span>" +
              "</td>" +
              "<td>" +
              '<button type="button" class="btn btn-sm btn-outline-discovery me-3 subtract-btn">-</button>' +
              '<span class="quantity">' +
              order.orderItemQuantity +
              "</span>" +
              '<button type="button" class="btn btn-sm btn-outline-discovery ms-3 add-btn">+</button>' +
              "</td>" +
              "<td>" +
              order.catName +
              "</td>" +
              "<td>₱" +
              '<span class="total_price">' +
              (order.orderItemQuantity * order.orderItemPrice).toFixed(2) +
              "</span>" +
              "</td>" +
              `<td><i class="fa-solid fa-trash btn text-danger delete-order" data-id="${order.orderItemId}"></i></td>` +
              "</tr>";
            tbody.append(row);
          });

          // Event delegation for dynamically added elements
          tbody.on("click", ".subtract-btn", function () {
            var $quantity = $(this).siblings(".quantity");
            var $row = $(this).closest("tr");
            var $price = $row.find(".price");
            var $totalPrice = $row.find(".total_price");

            var currentQuantity = parseInt($quantity.text());
            var currentPrice = parseFloat($price.text().replace("₱", ""));
            if (currentQuantity > 1) {
              var newQuantity = currentQuantity - 1;
              var newTotalPrice = (newQuantity * currentPrice).toFixed(2);

              $quantity.text(newQuantity);
              $totalPrice.text(newTotalPrice);

              updateOrder($row, newQuantity);
            }
          });

          tbody.on("click", ".add-btn", function () {
            var $quantity = $(this).siblings(".quantity");
            var $row = $(this).closest("tr");
            var $price = $row.find(".price");
            var $totalPrice = $row.find(".total_price");

            var currentQuantity = parseInt($quantity.text());
            var currentPrice = parseFloat($price.text().replace("₱", ""));
            var newQuantity = currentQuantity + 1;
            var newTotalPrice = (newQuantity * currentPrice).toFixed(2);

            $quantity.text(newQuantity);
            $totalPrice.text(newTotalPrice);

            updateOrder($row, newQuantity);
          });

          $(".delete-order")
            .off("click")
            .on("click", function () {
              var orderId = $(this).data("id");
              console.log(orderId);
              Swal.fire({
                title: "Are you sure?",
                text: "You won't be able to revert this!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#716add",
                cancelButtonColor: "#d33",
                confirmButtonText: "Yes, delete it!",
              }).then((result) => {
                if (result.isConfirmed) {
                  $.ajax({
                    url: "/api/Customer/getcustomer",
                    type: "GET",
                    dataType: "json",
                    data: { emailAddress: email },
                    success: function (data) {
                      $.ajax({
                        url: "/api/order/DeleteOrder/" + orderId,
                        method: "DELETE",
                        dataType: "json",
                        data: JSON.stringify({ orderItemId: orderId }),
                        success: function (data) {
                          console.log(data);
                        },
                      });
                    },
                  });
                  setTimeout(() => {
                    window.location.href = "/Home/Order";
                  }, 500);
                }
              });
            });
        },
      });
    },
  });

  function updateOrder(row, newQuantity) {
    var orderId = row.find(".delete-order").data("id");
    $.ajax({
      url: "/api/order/UpdateOrder/" + orderId + "/" + newQuantity,
      method: "PUT",
      dataType: "json",
      contentType: "application/json",
      data: JSON.stringify({
        orderId: orderId,
        orderQuantity: newQuantity,
      }),
      success: function (response) {
        var price = parseFloat(row.find("td:eq(2)").text().replace("₱", ""));
        row.find("td:eq(5)").text("₱" + (price * newQuantity).toFixed(2));
      },
      error: function (error) {
        // console.log("Error updating order", error);
      },
    });
  }
});
