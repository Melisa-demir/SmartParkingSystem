import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

function App() {
  const [parkingSpots, setParkingSpots] = useState([]);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [isLoggedIn, setIsLoggedIn] = useState(
    localStorage.getItem("token") !== null
  );

  const getParkingSpots = () => {
    const token = localStorage.getItem("token");

    fetch("https://localhost:7002/api/ParkingSports", {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
      .then(async (response) => {
        if (!response.ok) {
          throw new Error("Parking spots alınamadı: " + response.status);
        }

        return await response.json();
      })
      .then((data) => setParkingSpots(data))
      .catch((error) => console.error(error));
  };

  const toggleParkingSpot = (id) => {
    const token = localStorage.getItem("token");

    fetch(`https://localhost:7002/api/ParkingSports/${id}/toggle-status`, {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
      .then(async (response) => {
        if (!response.ok) {
          throw new Error("Park yeri güncellenemedi: " + response.status);
        }

        return await response.json();
      })
      .then(() => {
        getParkingSpots();
      })
      .catch((error) => console.error(error));
  };

  const login = () => {
    fetch(
      `https://localhost:7002/login?Email=${encodeURIComponent(email)}&Password=${encodeURIComponent(password)}`,
      {
        method: "POST"
      }
    )
      .then(async (response) => {
        const text = await response.text();

        if (!response.ok) {
          alert(text || "Login başarısız. Email veya şifre hatalı olabilir.");
          return;
        }

        const data = JSON.parse(text);

        localStorage.setItem("token", data.token);
        setIsLoggedIn(true);

        alert("Login başarılı");
        getParkingSpots();
      })
      .catch((error) => console.error(error));
  };

  const logout = () => {
    localStorage.removeItem("token");
    setIsLoggedIn(false);
    setParkingSpots([]);
  };

  useEffect(() => {
    if (!isLoggedIn) {
      return;
    }

    getParkingSpots();

    const connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7002/parkingHub")
      .withAutomaticReconnect()
      .build();

    connection
      .start()
      .then(() => console.log("SignalR bağlantısı kuruldu."))
      .catch((error) => console.error("SignalR bağlantı hatası:", error));

    connection.on("ParkingSpotUpdated", () => {
      console.log("Parking spot güncellendi.");
      getParkingSpots();
    });

    return () => {
      connection.stop();
    };
  }, [isLoggedIn]);

  if (!isLoggedIn) {
    return (
      <div>
        <h1>🚗 Smart Parking System</h1>

        <h2>Login</h2>

        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />

        <br />
        <br />

        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />

        <br />
        <br />

        <button onClick={login}>Login</button>
      </div>
    );
  }

  return (
    <div>
      <h1>🚗 Smart Parking System</h1>

      <button onClick={logout}>Logout</button>

      <h2>Parking Spots</h2>

      {parkingSpots.map((spot) => (
        <div key={spot.id}>
          <strong>{spot.spotNumber}</strong> -
          {spot.isOccupied ? " Occupied" : " Empty"}

          <button onClick={() => toggleParkingSpot(spot.id)}>
            {spot.isOccupied ? "Boş Yap" : "Dolu Yap"}
          </button>
        </div>
      ))}
    </div>
  );
}

export default App;