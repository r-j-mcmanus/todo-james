const form = document.getElementById("login-form");


form.addEventListener('submit', (event) => {
    event.preventDefault(); // prevents reloading on click

    const nameInput = form.elements["username"]
    const passwordInput = form.elements["password"]

    const loginObj = {
        'UserName': nameInput, 
        'Password': passwordInput
    }

    if (event.submitter.id == "login-btn") {
        postLogin(loginObj, 'login')
    }
    else if (event.submitter.id == "register-btn"){
        postLogin(loginObj, 'register')
    }
});


async function postLogin(postBody, endpoint) {
    const url = `http://localhost:5168/${endpoint}`
    
    try {
        const response = await fetch(url, 
            {
                method: "POST",
                body: JSON.stringify(postBody),
                headers: {
                    'Content-Type': 'application/json'
                }
            }
        );

        if (!response.ok) {
            throw new Error("Network response was not ok!", response)
        }

        const data = response.json();
        console.log('Successfully updated the server', data)

        fetchTodos()
    } catch (error) {
        console.log("there was an error", error)
    }
};
