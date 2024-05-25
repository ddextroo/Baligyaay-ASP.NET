$(document).ready(function () {
  $("#logout").click(function () {
    Swal.fire({
      title: "Confirm Logout",
      text: "Please confirm that you want to log out. After logout, you will need to log in again.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#d33",
      cancelButtonColor: "#716add",
      confirmButtonText: "Yes, Log Out",
    }).then((result) => {
      if (result.isConfirmed) {
        $.ajax({
          url: "/api/customer/AdminLogout",
          type: "POST",
          dataType: "json",
          success: function (data) {
            setTimeout(() => {
              window.location.href = "/Home/Admin";
            }, 500);
          },
        });
        setTimeout(() => {
          window.location.href = "/Home/Admin";
        }, 500);
      }
    });
  });
  function loadCustomers() {
    $.ajax({
      url: "/api/Customer",
      method: "GET",
      success: function (data) {
        var tbody = $("#CustomerTable tbody");
        tbody.empty();
        data.forEach(function (customer, index) {
          var row =
            "<tr>" +
            "<td>" +
            customer.id +
            "</td>" +
            "<td>" +
            customer.firstName +
            "</td>" +
            "<td>" +
            customer.lastName +
            "</td>" +
            "<td>" +
            customer.phone +
            "</td>" +
            "<td>" +
            customer.email +
            "</td>" +
            `<td><i class="fa-solid fa-trash btn text-danger delete-customer" data-id="${customer.id}"></i></td>` +
            "</tr>";
          tbody.append(row);
        });

        $(".delete-customer")
          .off("click")
          .on("click", function () {
            var customerId = $(this).data("id");
            deleteCustomer(customerId);
          });
      },
      error: function (err) {
        console.error("Error retrieving data:", err);
      },
    });
  }
  loadCustomers();

  function deleteCustomer(cusID) {
    $.ajax({
      url: "/api/Customer/Delete/" + cusID,
      method: "DELETE",
      dataType: "json",
      success: function (response) {
        Swal.fire({
          title: response,
          width: 600,
          padding: "3em",
          color: "#716add",
          background:
            "#fff url(https://sweetalert2.github.io/images/trees.png)",
          backdrop: `
                    rgba(0,0,123,0.4)
                    url("https://sweetalert2.github.io/images/nyan-cat.gif")
                    left top
                    no-repeat
                `,
        }).then((result) => {
          if (result.isConfirmed) {
            loadCustomers();
            setTimeout(() => {
              window.location.href = "/Home/CustomerAdmin";
            }, 500);
          }
        });
      },
    });
    setTimeout(() => {
      window.location.href = "/Home/CustomerAdmin";
    }, 500);
  }
});
