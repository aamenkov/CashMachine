import './App.css';
import {useCallback, useEffect, useState} from "react";
import update from "./update.png"
const App = () => {
  const [cassetteList, setCassetteList] = useState([{value: "0", amount: "0", isWorking: true}])
  const [total, setTotal] = useState(0)
  const [money, setMoney] = useState(0)
  const [account, setAccount] = useState(1000)
  const addCassette = () => {
    setCassetteList(prev => [...prev, {value: "0", amount: "0", isWorking: true}])
  }

  const onChangeDenomination = (num, event) => {
    const list = [...cassetteList]
    list[num].value = event.target.value
    setCassetteList(list)
  }

  const onChangeCount = (num, event) => {
    const list = [...cassetteList]
    list[num].amount = event.target.value
    setCassetteList(list)
  }

  const onChangeCheckBox = (num, { target: { checked } }) => {
    const list = [...cassetteList]
    list[num].isWorking = checked
    setCassetteList(list)
  }

  useEffect(() => {
    setTotal(cassetteList.reduce((sum, cassette) => {
      return sum + (cassette.isWorking ? (cassette.value | 0) * (cassette.amount | 0) : 0)
    }, 0))
  }, [cassetteList])

  useEffect(() => updateAccount, [])

  const getMoney = useCallback(async () => {
    try {
      if (money == (money | 0)){
        const url = "http://localhost:48790/api/ATM"
        const method = "POST"
        let body = JSON.stringify({money: +money})
        let headers = {"Content-Type": "application/json"}

        console.log(body)

        /*const response = await fetch(url, {method, body, headers})
		const data = await response.json()

		if (!response.ok) {
		  throw new Error(data.message || "Что-то пошло не так")
		}

		console.log(data)*/
      } else alert("Введите число")
    } catch (e) {
      if (e instanceof Error) {
        console.error(e.message)
      }
      throw e
    }
  }, [money])

  const updateAccount = useCallback(async () => {
    try {
      const url = "http://localhost:48790/api/ATM"
      const method = "GET"
      let headers = {"Content-Type": "application/json"}

      console.log("update account")
      /* const response = await fetch(url, {method, headers})
      const data = await response.json()*/

      /*if (!response.ok) {
        throw new Error(data.message || "Что-то пошло не так")
      }

      console.log(data)
      setAccount(data)*/
    } catch (e) {
      if (e instanceof Error) {
        console.error(e.message)
      }
      throw e
    }
  }, [])

  const createAtm = useCallback(async () => {
    try {


      const checkValue = [...cassetteList].filter(cassette => {
        return !(cassette.value === "10" || cassette.value === "50" || cassette.value === "100" || cassette.value === "200" || cassette.value === "500" || cassette.value === "1000" || cassette.value === "2000" || cassette.value === "5000")
      }).length

      const checkIsNumber = [...cassetteList].filter(cassette => {
        return !(cassette.value == (cassette.value | 0) && cassette.amount == (cassette.amount | 0) && cassette.amount !== "0")
      }).length

      if(!checkValue && !checkIsNumber) {
        console.log(cassetteList)
        const url = "http://localhost:48790/api/ATM"
        const method = "POST"
        let body = JSON.stringify({cassetteList})
        let headers = {"Content-Type": "application/json"}
        /* const response = await fetch(url, {method, body, headers})
		const data = await response.json()*/

        /*if (!response.ok) {
		  throw new Error(data.message || "Что-то пошло не так")
		}

		console.log(data)*/
      }

      if (checkValue) {
        alert("Номинал: 10, 50, 100, 200, 500, 1000, 2000, 5000")
      }
      if (checkIsNumber) {
        alert("Введите числа")
      }
    } catch (e) {
      if (e instanceof Error) {
        console.error(e.message)
      }
      throw e
    }
  }, [cassetteList])

  return (
    <div className="App">
      <h1>Банкомат</h1>
      <div className="container">
        <div className="setting">
          <h3>Настройка</h3>
          <div className="header flex">
            <span className="cell">Номинал:</span>
            <span className="cell">Количество:</span>
          </div>
          {cassetteList.map((cassette, num) => {
            return (
              <Cassete cassette={cassette} num={num} key={num} onChangeDenomination={onChangeDenomination} onChangeCount={onChangeCount} onChangeCheckBox={onChangeCheckBox}/>
            )
          })}
          <p>Итог: {total}</p>
          <div className="flex">
            <button className="cell" onClick={addCassette}>Добавить кассету</button>
            <button className="cell" onClick={createAtm}>Создать банкомат</button>
          </div>
        </div>
        <div className="money">
          <h3>Снять</h3>
          <span className="account">Счет: {account}<img src={update} onClick={updateAccount}/></span>
          <input className="keesf" value={money} onChange={event => setMoney(event.target.value)}/>
          <button onClick={getMoney}>Снять деньги</button>
        </div>
      </div>
    </div>
  );
}

const Cassete = ({cassette, num, onChangeDenomination, onChangeCount, onChangeCheckBox}) => {
  return (
    <>
      <div className="cassette flex">
        <input className="cell" value={cassette.value} onChange={event => onChangeDenomination(num, event)}/>
        <input className="cell" value={cassette.amount} onChange={event => onChangeCount(num, event)}/>
      </div>
      <input className="check" type="checkbox" onChange={event => onChangeCheckBox(num, event)} defaultChecked={true}/>
    </>
  )
}

export default App;
