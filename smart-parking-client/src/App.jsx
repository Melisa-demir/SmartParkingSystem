import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

const API_URL = "http://localhost:5005";

function App() {
  const [parkingSpots, setParkingSpots] = useState([]);
  const [reservations, setReservations] = useState([]);
  const [adminDashboard, setAdminDashboard] = useState(null);

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [isLoggedIn, setIsLoggedIn] = useState(
    localStorage.getItem("token") !== null
  );

  const [role, setRole] = useState(localStorage.getItem("role"));

  const isAdmin = role === "Admin";

  const getParkingSpots = () => {
    const token = localStorage.getItem("token");

    fetch(`${API_URL}/api/ParkingSports`, {
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

    fetch(`${API_URL}/api/Reservations`, {
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

        if (isAdmin) {
          setReservations(data);
        } else {
          const myReservations = data.filter(
            (reservation) => reservation.userId === userId
          );

          setReservations(myReservations);
        }
      })
      .catch((error) => console.error(error));
  };

  const getAdminDashboard = () => {
    const token = localStorage.getItem("token");

    fetch(`${API_URL}/api/Admin/dashboard`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
      .then(async (response) => {
        if (!response.ok) {
          throw new Error("Admin dashboard alınamadı: " + response.status);
        }

        return await response.json();
      })
      .then((data) => setAdminDashboard(data))
      .catch((error) => console.error(error));
  };

  const createReservation = (parkingSpotId) => {
    const token = localStorage.getItem("token");
    const userId = localStorage.getItem("userId");

    const startTime = new Date();
    const endTime = new Date();
    endTime.setHours(endTime.getHours() + 2);

    fetch(`${API_URL}/api/Reservations`, {
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

        if (isAdmin) {
          getAdminDashboard();
        }
      })
      .catch((error) => console.error(error));
  };

  const completeReservation = (id) => {
    const token = localStorage.getItem("token");

    fetch(`${API_URL}/api/Reservations/${id}/complete`, {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
      .then(async (response) => {
        const text = await response.text();

        if (!response.ok) {
          alert(text || "Rezervasyon tamamlanamadı.");
          return;
        }

        alert("Rezervasyon tamamlandı.");
        getReservations();
        getParkingSpots();

        if (isAdmin) {
          getAdminDashboard();
        }
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
        fetch(`${API_URL}/api/Reservations/${reservation.id}`, {
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

        if (isAdmin) {
          getAdminDashboard();
        }
      })
      .catch((error) => console.error(error));
  };

  const toggleParkingSpot = (id) => {
    const token = localStorage.getItem("token");

    fetch(`${API_URL}/api/ParkingSports/${id}/toggle-status`, {
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

        if (isAdmin) {
          getAdminDashboard();
        }
      })
      .catch((error) => console.error(error));
  };

  const login = () => {
    fetch(
      `${API_URL}/login?Email=${encodeURIComponent(email)}&Password=${encodeURIComponent(password)}`,
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
        localStorage.setItem("role", data.user.role);

        setRole(data.user.role);
        setIsLoggedIn(true);

        alert("Login başarılı");
      })
      .catch((error) => console.error(error));
  };

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("userId");
    localStorage.removeItem("role");

    setIsLoggedIn(false);
    setRole(null);
    setParkingSpots([]);
    setReservations([]);
    setAdminDashboard(null);
  };

  useEffect(() => {
    if (!isLoggedIn) {
      return;
    }

    getParkingSpots();
    getReservations();

    if (isAdmin) {
      getAdminDashboard();
    }

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_URL}/parkingHub`)
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

      if (isAdmin) {
        getAdminDashboard();
      }
    });

    return () => {
      connection.stop();
    };
  }, [isLoggedIn, role]);

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

      <p>
        Giriş yapan rol: <strong>{role}</strong>
      </p>

      <button onClick={logout}>Logout</button>

      <h2>Dashboard</h2>

      {isAdmin && adminDashboard && (
        <div>
          <h3>Admin Dashboard</h3>

          <p>Toplam Kullanıcı: {adminDashboard.totalUsers}</p>
          <p>Toplam Rezervasyon: {adminDashboard.totalReservations}</p>
          <p>Aktif Rezervasyon: {adminDashboard.activeReservations}</p>
          <p>Tamamlanan Rezervasyon: {adminDashboard.completedReservations}</p>
          <p>Toplam Park Yeri: {adminDashboard.totalParkingSpots}</p>
          <p>Dolu Park Yeri: {adminDashboard.occupiedParkingSpots}</p>
          <p>Boş Park Yeri: {adminDashboard.emptyParkingSpots}</p>

          <hr />
        </div>
      )}

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

          {isAdmin && (
            <button onClick={() => toggleParkingSpot(spot.id)}>
              {spot.isOccupied ? "Boş Yap" : "Dolu Yap"}
            </button>
          )}
        </div>
      ))}

      <h2>{isAdmin ? "All Reservations" : "My Reservations"}</h2>

      {isAdmin && reservations.length > 0 && (
        <button onClick={deleteAllReservations}>
          Tüm Rezervasyonları Sil
        </button>
      )}

      {reservations.length === 0 && <p>Henüz rezervasyon bulunmuyor.</p>}

      {reservations.map((reservation) => (
        <div key={reservation.id}>
          <strong>{reservation.parkingSpot?.spotNumber}</strong>

          <br />

          {isAdmin && (
            <>
              User Id: {reservation.userId}
              <br />
            </>
          )}

          Start: {new Date(reservation.startTime).toLocaleString()}

          <br />

          End: {new Date(reservation.endTime).toLocaleString()}

          <br />

          Price: {reservation.totalPrice} TL

          <br />

          Status: {reservation.status}

          {reservation.status === "Active" && (
            <>
              <br />
              <button onClick={() => completeReservation(reservation.id)}>
                Rezervasyonu Tamamla
              </button>
            </>
          )}

          <hr />
        </div>
      ))}
    </div>
  );
}

export default App;