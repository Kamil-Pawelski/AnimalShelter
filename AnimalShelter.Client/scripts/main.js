function loadHeader() {
  fetch('../nav/nav.html')
    .then((response) => response.text())
    .then((data) => {
      document.getElementById('header').innerHTML = data;
      updateNavLinks();
    })
    .catch((error) => {
      console.error('Error loading the header:', error);
    });
}

function updateNavLinks() {
  if (localStorage.getItem('token')) {
    console.log(document.getElementById('login-link'));
    document.getElementById('login-link').style.display = 'none';
    document.getElementById('register-link').style.display = 'none';
    document.getElementById('logout-link').style.display = 'inline';

    document
      .getElementById('logout-link')
      .addEventListener('click', function () {
        localStorage.removeItem('token');
        window.location.href = '/index.html';
      });
  } else {
    document.getElementById('login-link').style.display = 'inline';
    document.getElementById('register-link').style.display = 'inline';
    document.getElementById('logout-link').style.display = 'none';
  }
}

document.addEventListener('DOMContentLoaded', loadHeader);
