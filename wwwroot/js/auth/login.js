$(document).ready(function () {
  $("#login").on("click", function () {
    var email = $("#email").val();
    var password = $("#pass").val();

    $.ajax({
      url: "/api/Customer/Login",
      type: "POST",
      contentType: "application/json",
      data: JSON.stringify({ email: email, password: password }),
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
        }).then((result) => {
          if (result.isConfirmed) {
            window.location.href = "/Home";
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
