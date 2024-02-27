// Function for dark/light mode
let isFirstImage = true;
function darkLight() {
    var element = document.body;
    var img = document.querySelector("#moonImg");
    element.classList.toggle("darkMode");
    if (isFirstImage) {
        img.src = "../../Images/sun.png";
    }
    else {
        img.src = "../../Images/moon.svg";
    }
    isFirstImage = !isFirstImage;
}
