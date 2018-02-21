// Write your Javascript code.

$("#nav-shopping-cart").on("click", function (evt) {
  evt.preventDefault();
  $(".minicart").slideToggle(333);
})
