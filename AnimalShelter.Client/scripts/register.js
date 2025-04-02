document
  .getElementById('register-form')
  .addEventListener('submit', function (event) {
    event.preventDefault();

    const username = document.getElementById('username').value;
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    fetch('https://localhost:7000/register', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        username: username,
        email: email,
        password: password,
      }),
    })
      .then((response) => response.json())
      .then((data) => {
        window.location.href = '/pages/login.html';
      })
      .catch((error) => {
        console.error('Error registering:', error);
      });
  });
