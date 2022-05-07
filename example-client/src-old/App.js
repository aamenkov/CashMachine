import logo from './logo.svg';
import './App.css';
import { Component } from 'react';

class App extends Component {
  constructor(){
    super();
    this.state = {
      users: []
    }
  }

  getUsers = async () => {
    var response = await fetch (
      'http://localhost:48790/api/users',
      {
        method: 'get'
      }
    )  
    
    var responsejson = await response.json();
    this.setState({
      users: responsejson
    })

    console.log(responsejson);
  }

  render() {
    const users = this.state.users.map((item, index) => <li key={index}>{item.name}</li>)
    return (
      <div classname='App'>
        <button onClick={this.getUsers}>Загрузить список пользователей</button>
        <button >Попытка 13 </button>
        <ul>{users}</ul>
      </div>
    );
  }
}

export default App;