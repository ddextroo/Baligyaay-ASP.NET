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
  $("#DataModal3 .modal-footer button:nth-child(2)")
    .off("click")
    .on("click", function () {
      var category_name = $("#category_name").val();
      $.ajax({
        url: "/api/Product/Category",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ name: category_name }),
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },
        success: function (data) {
          $.ajax({
            url: "/api/Product/Category",
            type: "GET",
            dataType: "json",
            headers: {
              Accept: "application/json",
              "Content-Type": "application/json",
            },
            success: function (data) {},
            error: function (jqXHR, textStatus, errorThrown) {
              console.log(textStatus, errorThrown);
            },
          });
          $("#category_name").val("");
          Swal.fire({
            title: data,
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
              setTimeout(() => {
                window.location.href = "/Home/Admin";
              }, 500);
            }
          });
        },
        error: function (xhr, status, error) {
          console.error(xhr.responseText);
          Swal.fire({
            icon: "error",
            title: "Oops...",
            text: xhr.responseText,
          });
        },
      });
    });
});
