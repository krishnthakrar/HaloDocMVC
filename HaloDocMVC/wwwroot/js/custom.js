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

// Function for Upload file

function displayFilename() {
    var input = document.getElementById('myFile');
    var output = document.getElementById('selectedFilename');
    output.textContent = input.files[0].name;
}