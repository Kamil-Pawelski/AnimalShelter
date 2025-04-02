document
  .getElementById('login-form')
  .addEventListener('submit', function (event) {
    event.preventDefault();

    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    fetch('https://localhost:7000/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        username: username,
        password: password,
      }),
    })
      .then((response) => response.json())
      .then((data) => {
        localStorage.setItem('token', data);
        console.log(data);
        window.location.href = '/index.html';
      })
      .catch((error) => {
        console.error('Error logging in:', error);
        alert('An error occurred while logging in.');
      });
  });
