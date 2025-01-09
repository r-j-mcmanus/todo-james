const form = document.querySelector("#todo-form");
const list = document.querySelector("#todo-list");

let TODOs = [];

if (localStorage["data"] !== null && localStorage["data"] !== undefined) 
{
    TODOs = JSON.parse(localStorage["data"]);
}

function buildUI() {
    let HTML = ``;
    TODOs.forEach((todo) =>
    {
        isDone = todo.IsComplete ? "x" : "o"

        HTML += `
            <li id="${todo.Id}">
                <span> ${todo.Name} </span>
                <button class="button-complete">
                ${isDone}
                </button>
            </li>
        `
    }
    );

    list.innerHTML = HTML;
}

async function fetchTodos() {
    const url = "http://localhost:5168/todoitems";
    
    try {
        const response = await fetch(url, {method: "GET"});

        if (!response.ok) {
            throw new Error("Network response was not ok!", response)
        };

        TODOs = await response.json();
        buildUI()

    } catch (error) {
        console.log("there was an error when fetching todos", error)
        throw error
    }
}

async function postTodo(todo) {
    const url = "http://localhost:5168/todoitems"
    
    try {
        const response = await fetch(url, 
            {
                method: "POST",
                body: JSON.stringify(todo),
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
        //throw error
    }
};


form.addEventListener("submit", (event) => 
{
    event.preventDefault(); // prevents reloading on click

    const todo = {
        Name: event.target[0].value, 
        IsComplete: false,
        ref: self.crypto.randomUUID() // universally unique id
    }

    // add an element to todo data storage
    TODOs.push(todo);

    localStorage["data"] = JSON.stringify(TODOs);

    // render new element
    buildUI();

    postTodo(todo);

    form.reset(); // remove filled values form the form
}
);

buildUI();
try {
    fetchTodos()
} catch (err) {
    console.log("there was an error on initial load", err)
}
