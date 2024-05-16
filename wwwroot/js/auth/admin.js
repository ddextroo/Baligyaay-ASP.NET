$(document).ready(function () {
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
