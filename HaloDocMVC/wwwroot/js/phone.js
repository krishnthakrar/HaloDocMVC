// Function for Phone number field

function phone1() {
    const phoneInputField = document.querySelector("#phone");
    const phoneInput = window.intlTelInput(phoneInputField, {
        initialCountry: "auto",
        geoIpLookup: callback => {
            fetch("https://ipapi.co/json")
                .then(res => res.json())
                .then(data => callback(data.country_code))
                .catch(() => callback("us"));
        },
        separateDialCode: true,
        hiddenInput: "full",
        utilsScript: "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
    });
    const input = document.querySelector("#phone");
    const button = document.querySelector("#phone");
    const errorMsg = document.querySelector("#error-msg");
    const errorMap = ["Invalid number", "Invalid country code", "Too short", "Too long", "Invalid number"];

    const reset = () => {
        input.classList.remove("error");
        errorMsg.innerHTML = "";
        errorMsg.classList.add("hide");
    };

    const showError = (msg) => {
        input.classList.add("error");
        errorMsg.innerHTML = msg;
        errorMsg.classList.remove("hide");
    };

    button.addEventListener('focusout', (event) => {

        reset();

        if (!input.value.trim()) {
            showError("Required");

        } else {
            // Assume 'e' is a validation object/library with methods isValidNumber() and getValidationError()
            if (phoneInput.isValidNumber(input.value)) {

                const full_number = phoneInput.getNumber(intlTelInputUtils.numberFormat.E164);
                console.log(full_number);
                document.getElementById("phone").value = full_number;

            } else {
                const errorCode = phoneInput.getValidationError(input.value);
                const msg = errorMap[errorCode] || "Invalid number";
                showError(msg);
            }
        }
    });

    // Reset error state on input change
    input.addEventListener('change', reset);
    input.addEventListener('keyup', reset);
}
function phone2() {
    const phoneInputField = document.querySelector("#phone");
    const phoneInput = window.intlTelInput(phoneInputField, {
        initialCountry: "auto",
        geoIpLookup: callback => {
            fetch("https://ipapi.co/json")
                .then(res => res.json())
                .then(data => callback(data.country_code))
                .catch(() => callback("us"));
        },
        separateDialCode: true,
        hiddenInput: "full",
        utilsScript: "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
    });

    const phoneInputField1 = document.querySelector("#phone1");
    const phoneInput1 = window.intlTelInput(phoneInputField1, {
        initialCountry: "auto",
        geoIpLookup: callback => {
            fetch("https://ipapi.co/json")
                .then(res => res.json())
                .then(data => callback(data.country_code))
                .catch(() => callback("us"));
        },
        separateDialCode: true,
        hiddenInput: "full",
        utilsScript: "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
    });
    const input = document.querySelector("#phone");
    const button = document.querySelector("#phone");
    const errorMsg = document.querySelector("#error-msg");
    const input1 = document.querySelector("#phone1");
    const button1 = document.querySelector("#phone1");
    const errorMsg1 = document.querySelector("#error-msg1");
    const errorMap1 = ["Invalid number", "Invalid country code", "Too short", "Too long", "Invalid number"];
    const reset = () => {
        input.classList.remove("error");
        errorMsg.innerHTML = "";
        errorMsg.classList.add("hide");
    };
    const reset1 = () => {
        input1.classList.remove("error");
        errorMsg1.innerHTML = "";
        errorMsg1.classList.add("hide");
    }
    const showError1 = (msg) => {
        input1.classList.add("error");
        errorMsg1.innerHTML = msg;
        errorMsg1.classList.remove("hide");
    };
    const showError = (msg) => {
        input.classList.add("error");
        errorMsg.innerHTML = msg;
        errorMsg.classList.remove("hide");
    };


    button.addEventListener('focusout', (event) => {
        var flag = false;
        var flag1 = false;
        reset();

        if (!input1.value.trim()) {
            showError1("Required");
            document.getElementById('submit').setAttribute("disabled", "disabled");
            flag = false;
        } else {
            // Assume 'e' is a validation object/library with methods isValidNumber() and getValidationError()
            if (phoneInput1.isValidNumber(input1.value)) {

                const full_number1 = phoneInput1.getNumber(intlTelInputUtils.numberFormat.E164);
                console.log(full_number1);
                document.getElementById("phone1").value = full_number1;
                flag = true;

            } else {
                const errorCode1 = phoneInput1.getValidationError(input1.value);
                const msg1 = errorMap1[errorCode1] || "Invalid number";
                showError1(msg1)
                document.getElementById('submit').setAttribute("disabled", "disabled");
                flag = false;
            }
        }

        if (!input.value.trim()) {
            showError("Required");
            document.getElementById('submit').setAttribute("disabled", "disabled");
            flag = false;
        } else {
            // Assume 'e' is a validation object/library with methods isValidNumber() and getValidationError()
            if (phoneInput.isValidNumber(input.value)) {

                const full_number = phoneInput.getNumber(intlTelInputUtils.numberFormat.E164);
                console.log(full_number);
                document.getElementById("phone").value = full_number;
                flag1 = true;

            } else {
                const errorCode = phoneInput.getValidationError(input.value);
                const msg = errorMap1[errorCode] || "Invalid number";
                showError(msg);
                document.getElementById('submit').setAttribute("disabled", "disabled");
                flag = false;
            }
        }
        console.log(flag);
        console.log(flag1);
        if (flag == true && flag1 == true) {
            document.getElementById('submit').removeAttribute("disabled");
        }
    });

    button1.addEventListener('focusout', (event) => {
        var flag = false;
        var flag1 = false;
        reset();

        if (!input1.value.trim()) {
            showError1("Required");
            document.getElementById('submit').setAttribute("disabled", "disabled");
            flag = false;
        } else {
            // Assume 'e' is a validation object/library with methods isValidNumber() and getValidationError()
            if (phoneInput1.isValidNumber(input1.value)) {

                const full_number1 = phoneInput1.getNumber(intlTelInputUtils.numberFormat.E164);
                console.log(full_number1);
                document.getElementById("phone1").value = full_number1;
                flag = true;

            } else {
                const errorCode1 = phoneInput1.getValidationError(input1.value);
                const msg1 = errorMap1[errorCode1] || "Invalid number";
                showError1(msg1)
                document.getElementById('submit').setAttribute("disabled", "disabled");
                flag = false;
            }
        }

        if (!input.value.trim()) {
            showError("Required");
            document.getElementById('submit').setAttribute("disabled", "disabled");
            flag = false;
        } else {
            // Assume 'e' is a validation object/library with methods isValidNumber() and getValidationError()
            if (phoneInput.isValidNumber(input.value)) {

                const full_number = phoneInput.getNumber(intlTelInputUtils.numberFormat.E164);
                console.log(full_number);
                document.getElementById("phone").value = full_number;
                flag1 = true;

            } else {
                const errorCode = phoneInput.getValidationError(input.value);
                const msg = errorMap1[errorCode] || "Invalid number";
                showError(msg);
                document.getElementById('submit').setAttribute("disabled", "disabled");
                flag = false;
            }
        }
        console.log(flag);
        console.log(flag1);
        if (flag == true && flag1 == true) {
            document.getElementById('submit').removeAttribute("disabled");
        }
    });

    // on keyup / change flag: reset
    input1.addEventListener('change', reset1);
    input1.addEventListener('keyup', reset1);
    input.addEventListener('change', reset);
    input.addEventListener('keyup', reset);



}
