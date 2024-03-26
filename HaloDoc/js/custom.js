let isFirstImage = true;
// Function for Onload event
// swal({
//     title: "Information",
//     text: "When submitting a request, you must provide the correct contact information for the patient or the responsibly party. Failure to provide the correct email and phone number will delay service or be declined.",
//     type: "warning",
//     confirmButtonColor: "#0dcaf0",
    
// });

// Function to show/hide the password
function pwdShowHide() {
    var x = document.getElementById("password");
    if (x.type === "password") {
        x.type = "text";
        document.querySelectorAll(".showImg").forEach(Image => Image.style.display = "none");
        document.querySelectorAll(".hideImg").forEach(Image => Image.style.display = "block");
    } 
    else {
        x.type = "password";
        document.querySelectorAll(".hideImg").forEach(Image => Image.style.display = "none");
        document.querySelectorAll(".showImg").forEach(Image => Image.style.display = "block");
    }
}

// Function for dark/light mode

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

// Function for Phone number field

const phoneInputField = document.querySelector("#phone");
const phoneInput = window.intlTelInput(phoneInputField, {
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
});

const phoneInputField1 = document.querySelector("#uphone");
const phoneInput1 = window.intlTelInput(phoneInputField1, {
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
});

// Function for Upload file

function displayFilename() {
    var input = document.getElementById('myFile');
    var output = document.getElementById('selectedFilename');
    output.textContent = input.files[0].name;
}

function openTab(evt, tabName) {
    evt.preventDefault();
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(tabName).style.display = "block";
    evt.currentTarget.className += " active";
}