import './App.css';

function App() {
  return (
    <main className="app-shell">
      <section className="welcome" aria-labelledby="welcome-title">
        <h1 id="welcome-title">MyEnglish</h1>
        <p>
          React frontend is ready. Build the learning experience here and connect it to your ASP.NET API when the
          endpoints are ready.
        </p>
        <div className="actions">
          <a href="https://vite.dev/guide/" target="_blank" rel="noreferrer">
            Vite docs
          </a>
          <a className="secondary" href="https://react.dev/" target="_blank" rel="noreferrer">
            React docs
          </a>
        </div>
      </section>
    </main>
  );
}

export default App;

