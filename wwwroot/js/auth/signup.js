$(document).ready(function () {
  $("#create").on("click", function () {
    $.ajax({
      type: "POST",
      url: "/api/Customer",
      data: JSON.stringify({
        firstName: $("#fname").val(),
        lastName: $("#lname").val(),
        phone: $("#phone").val(),
        email: $("#email").val(),
        password: $("#pass").val(),
      }),
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
        console.log(xhr.responseText);
        Swal.fire({
          icon: "error",
          title: "Oops...",
          text: xhr.responseText,
        });
      },
    });
  });
});
