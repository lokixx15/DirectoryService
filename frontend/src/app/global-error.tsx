"use client";

export default function GlobalError({
  error,
  reset,
}: {
  error: Error;
  reset: () => void;
}) {
  return (
    <html style={{ margin: 0 }}>
      <body style={{ margin: 0, fontFamily: "system-ui, sans-serif" }}>
        <div
          style={{
            display: "flex",
            minHeight: "100vh",
            flexDirection: "column",
            alignItems: "center",
            justifyContent: "center",
            gap: "16px",
            padding: "32px",
            backgroundColor: "#fafafa",
            color: "#1a1a1a",
          }}
        >
          <div
            style={{
              display: "flex",
              flexDirection: "column",
              alignItems: "center",
              gap: "8px",
              textAlign: "center",
            }}
          >
            <h1 style={{ margin: 0, fontSize: "1.25rem", fontWeight: 600 }}>
              Critical error
            </h1>
            <p
              style={{
                margin: 0,
                fontSize: "0.875rem",
                color: "#b91c1c",
                backgroundColor: "#fef2f2",
                border: "1px solid #fca5a5",
                borderRadius: "8px",
                padding: "12px",
                maxWidth: "480px",
              }}
            >
              {error.message}
            </p>
          </div>
          <button
            onClick={reset}
            style={{
              padding: "8px 16px",
              fontSize: "0.875rem",
              fontWeight: 500,
              color: "#fff",
              backgroundColor: "#dc2626",
              border: "none",
              borderRadius: "6px",
              cursor: "pointer",
            }}
          >
            Retry page
          </button>
        </div>
      </body>
    </html>
  );
}
