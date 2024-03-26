function displayFilename() {
    var input = document.getElementById('myFile');
    var output = document.getElementById('selectedFilename');
    output.textContent = input.files[0].name;
}

function openTab(tabId) {
    // Hide all tab contents
    var tabContents = document.getElementsByClassName('tab-content');
    for (var i = 0; i < tabContents.length; i++) {
        tabContents[i].style.display = 'none';
    }

    // Remove 'active-tab' class from all tabs
    var tabs = document.getElementsByClassName('tab');
    for (var i = 0; i < tabs.length; i++) {
        tabs[i].classList.remove('active-tab');
    }

    // Show the selected tab content and mark it as active
    document.getElementById(tabId).style.display = 'block';
    document.querySelector('[onclick="openTab(\'' + tabId + '\')"]').classList.add('active-tab');
}

function sideBar() {
    var temp=document.getElementById('sidebar');
    var dissewtting=temp.style.display;
    if(dissewtting==='block')
    {
        temp.style.display='none';
        document.getElementById('content').style.width="190%";
    }
    else
    {
        temp.style.display='block';
        document.getElementById('content').style.width="190%";
    }
}