function loadHeader() {
  fetch('../nav/nav.html')
    .then((response) => response.text())
    .then((data) => {
      document.getElementById('header').innerHTML = data;
    })
    .catch((error) => {
      console.error('Error loading the header:', error);
    });
}

document.addEventListener('DOMContentLoaded', loadHeader);
