import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

function App() {
  const [parkingSpots, setParkingSpots] = useState([]);
  const [reservations, setReservations] = useState([]);
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

  const getReservations = () => {
    const token = localStorage.getItem("token");

    fetch("https://localhost:7002/api/Reservations", {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
      .then(async (response) => {
        if (!response.ok) {
          throw new Error("Reservations alınamadı: " + response.status);
        }

        return await response.json();
      })
      .then((data) => {
        const userId = Number(localStorage.getItem("userId"));

        const myReservations = data.filter(
          (reservation) => reservation.userId === userId
        );

        setReservations(myReservations);
      })
      .catch((error) => console.error(error));
  };

  const createReservation = (parkingSpotId) => {
    const token = localStorage.getItem("token");
    const userId = localStorage.getItem("userId");

    const startTime = new Date();
    const endTime = new Date();
    endTime.setHours(endTime.getHours() + 2);

    fetch("https://localhost:7002/api/Reservations", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify({
        userId: Number(userId),
        parkingSpotId: parkingSpotId,
        startTime: startTime.toISOString(),
        endTime: endTime.toISOString()
      })
    })
      .then(async (response) => {
        const text = await response.text();

        if (!response.ok) {
          alert(text || "Rezervasyon oluşturulamadı.");
          return;
        }

        alert("Rezervasyon oluşturuldu.");
        getParkingSpots();
        getReservations();
      })
      .catch((error) => console.error(error));
  };

  const deleteAllReservations = () => {
    const token = localStorage.getItem("token");

    if (!window.confirm("Tüm rezervasyonları silmek istediğine emin misin?")) {
      return;
    }

    Promise.all(
      reservations.map((reservation) =>
        fetch(`https://localhost:7002/api/Reservations/${reservation.id}`, {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${token}`
          }
        })
      )
    )
      .then(() => {
        alert("Tüm rezervasyonlar silindi.");
        getReservations();
        getParkingSpots();
      })
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
        getReservations();
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
        localStorage.setItem("userId", data.user.id);

        setIsLoggedIn(true);

        alert("Login başarılı");
        getParkingSpots();
        getReservations();
      })
      .catch((error) => console.error(error));
  };

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("userId");

    setIsLoggedIn(false);
    setParkingSpots([]);
    setReservations([]);
  };

  useEffect(() => {
    if (!isLoggedIn) {
      return;
    }

    getParkingSpots();
    getReservations();

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
      getReservations();
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

      <h2>Dashboard</h2>

      <p>Toplam Park Yeri: {parkingSpots.length}</p>

      <p>
        Boş Park Yeri:{" "}
        {parkingSpots.filter((spot) => !spot.isOccupied).length}
      </p>

      <p>
        Dolu Park Yeri:{" "}
        {parkingSpots.filter((spot) => spot.isOccupied).length}
      </p>

      <h2>Parking Spots</h2>

      {parkingSpots.map((spot) => (
        <div key={spot.id}>
          <strong>{spot.spotNumber}</strong> -
          {spot.isOccupied ? " Occupied" : " Empty"}

          {!spot.isOccupied && (
            <button onClick={() => createReservation(spot.id)}>
              Rezervasyon Yap
            </button>
          )}

          <button onClick={() => toggleParkingSpot(spot.id)}>
            {spot.isOccupied ? "Boş Yap" : "Dolu Yap"}
          </button>
        </div>
      ))}

      <h2>My Reservations</h2>

      {reservations.length > 0 && (
        <button onClick={deleteAllReservations}>
          Tüm Rezervasyonları Sil
        </button>
      )}

      {reservations.length === 0 && <p>Henüz rezervasyon bulunmuyor.</p>}

      {reservations.map((reservation) => (
        <div key={reservation.id}>
          <strong>{reservation.parkingSpot?.spotNumber}</strong>

          <br />

          Start: {new Date(reservation.startTime).toLocaleString()}

          <br />

          End: {new Date(reservation.endTime).toLocaleString()}

          <br />

          Price: {reservation.totalPrice} TL

          <br />

          Status: {reservation.status}

          <hr />
        </div>
      ))}
    </div>
  );
}

export default App;