import React, { useRef, useState, useEffect } from "react";

// Types
type UploadedFile = {
  name: string;
  file?: File;
};

type SearchResult = {
  id: string;
  name: string;
  pageNumbers: string;
  text: string;
};

type EnvConfig = {
  BASE_URL: string;
};

// Components
const UploadPDF: React.FC<{
  fileName: string | null;
  error: string | null;
  fileInputRef: React.RefObject<HTMLInputElement>;
  onFileChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
}> = ({ fileName, error, fileInputRef, onFileChange }) => (
  <div style={{
    background: "#fff",
    borderRadius: 12,
    boxShadow: "0 2px 8px rgba(0,0,0,0.07)",
    padding: 32,
    marginBottom: 24
  }}>
    <h2 style={{ marginBottom: 20, color: "#2d3748" }}>Upload a PDF File</h2>
    <input
      ref={fileInputRef}
      type="file"
      accept="application/pdf"
      onChange={onFileChange}
      style={{
        padding: "8px 0",
        marginBottom: 16,
        fontSize: 16
      }}
    />
    {fileName && <p style={{ color: "#3182ce", margin: 8 }}>Selected file: {fileName}</p>}
    {error && <p style={{ color: "#e53e3e", margin: 8 }}>{error}</p>}
  </div>
);

const UploadedPDFList: React.FC<{ uploadedFiles: UploadedFile[] }> = ({ uploadedFiles }) => (
  <div style={{
    background: "#fff",
    borderRadius: 12,
    boxShadow: "0 2px 8px rgba(0,0,0,0.07)",
    padding: 32,
    marginBottom: 24
  }}>
    <h2 style={{ marginBottom: 20, color: "#2d3748" }}>Uploaded PDFs</h2>
    {uploadedFiles.length === 0 ? (
      <p style={{ color: "#718096" }}>No PDFs uploaded yet.</p>
    ) : (
      <ul style={{ textAlign: "left", paddingLeft: 24 }}>
        {uploadedFiles.map((f, idx) => (
          <li key={idx} style={{
            padding: "6px 0",
            borderBottom: idx !== uploadedFiles.length - 1 ? "1px solid #edf2f7" : "none"
          }}>{f.name}</li>
        ))}
      </ul>
    )}
  </div>
);

const SearchResultsTable: React.FC<{ results: SearchResult[] }> = ({ results }) => (
  <div style={{
    background: "#fff",
    borderRadius: 12,
    boxShadow: "0 2px 8px rgba(0,0,0,0.07)",
    padding: 32,
    marginBottom: 24
  }}>
    <h2 style={{ marginBottom: 20, color: "#2d3748" }}>Search Results</h2>
    {results.length === 0 ? (
      <p style={{ color: "#718096" }}>No results found.</p>
    ) : (
      <div style={{ overflowX: "auto" }}>
        <table style={{
          width: "100%",
          borderCollapse: "collapse",
          background: "#f7fafc"
        }}>
          <thead>
            <tr>
              <th style={tableHeaderStyle}>Document ID</th>
              <th style={tableHeaderStyle}>Document Name</th>
              <th style={tableHeaderStyle}>Page</th>
              <th style={tableHeaderStyle}>Text</th>
            </tr>
          </thead>
          <tbody>
            {results.map((result, idx) => (
              <tr key={idx} style={{ background: idx % 2 === 0 ? "#fff" : "#f1f5f9" }}>
                <td style={tableCellStyle}>{result.id}</td>
                <td style={tableCellStyle}>{result.name}</td>
                <td style={tableCellStyle}>{result.pageNumbers}</td>
                <td style={{ ...tableCellStyle, textAlign: "left" }}>{result.text}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    )}
  </div>
);

const tableHeaderStyle: React.CSSProperties = {
  border: "1px solid #e2e8f0",
  padding: 8,
  background: "#3182ce",
  color: "#fff",
  fontWeight: 600,
  fontSize: 16
};

const tableCellStyle: React.CSSProperties = {
  border: "1px solid #e2e8f0",
  padding: 8,
  fontSize: 15,
  color: "#2d3748"
};

const navButtonStyle = (active: boolean): React.CSSProperties => ({
  background: active ? "#3182ce" : "#e2e8f0",
  color: active ? "#fff" : "#2d3748",
  border: "none",
  borderRadius: 6,
  padding: "10px 20px",
  marginRight: 10,
  fontSize: 16,
  fontWeight: 500,
  cursor: active ? "default" : "pointer",
  boxShadow: active ? "0 2px 6px rgba(49,130,206,0.12)" : undefined,
  transition: "all 0.2s"
});

const App: React.FC = () => {
  const [fileName, setFileName] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [uploadedFiles, setUploadedFiles] = useState<UploadedFile[]>([]);
  const [searchResults, setSearchResults] = useState<SearchResult[]>([]);
  const [page, setPage] = useState<"upload" | "list" | "search">("upload");
  const [env, setEnv] = useState<EnvConfig | null>(null);
  const [searchQuery, setSearchQuery] = useState<string>("");
  const fileInputRef = useRef<HTMLInputElement>(null);

  // Load env.json on mount
  useEffect(() => {
    fetch("/env.json")
      .then(res => res.json())
      .then(setEnv)
      .catch(() => setEnv({ BASE_URL: "http://localhost:5000" }));
  }, []);

  // Fetch files from backend
  const fetchFiles = async () => {
    if (!env) return;
    try {
      const res = await fetch(`${env.BASE_URL}/api/PdfDoc/list`);
      const data = await res.json();
      setUploadedFiles(data.files || []);
    } catch {
      setError("Failed to fetch files.");
    }
  };

  // Fetch search results from backend
  const fetchSearchResults = async (query: string) => {
    if (!env) return;
    try {
      const res = await fetch(`${env.BASE_URL}/api/PdfDoc/search`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ query })
      });
      const data = await res.json();
      setSearchResults(data.files || []);
    } catch {
      setError("Failed to fetch search results.");
    }
  };

  // Handle file upload to backend
  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setError(null);
    const file = e.target.files?.[0];
    if (file) {
      if (file.type !== "application/pdf") {
        setError("Only PDF files are allowed.");
        setFileName(null);
        if (fileInputRef.current) fileInputRef.current.value = "";
      } else {
        setFileName(file.name);
        if (env) {
          const formData = new FormData();
          formData.append("file", file);
          fetch(`${env.BASE_URL}/api/PdfDoc/index`, {
            method: "POST",
            body: formData,
          })
            .then(async (res) => {
              if (!res.ok) throw new Error("Upload failed");
              await fetchFiles();
            })
            .catch(() => setError("Failed to upload file."));
        }
      }
    }
  };

  return (
    <div style={{
      minHeight: "100vh",
      background: "linear-gradient(135deg, #e0e7ff 0%, #f0fff4 100%)",
      padding: "60px 0"
    }}>
      <div style={{
        maxWidth: 700,
        margin: "0 auto",
        background: "rgba(255,255,255,0.85)",
        borderRadius: 16,
        boxShadow: "0 4px 24px rgba(0,0,0,0.08)",
        padding: "40px 32px"
      }}>
        <div style={{ marginBottom: 32, textAlign: "center" }}>
          <button
            onClick={() => setPage("upload")}
            disabled={page === "upload"}
            style={navButtonStyle(page === "upload")}
          >
            Upload PDF
          </button>
          <button
            onClick={() => {
              setPage("list");
              fetchFiles();
            }}
            disabled={page === "list"}
            style={navButtonStyle(page === "list")}
          >
            List PDFs
          </button>
          <button
            onClick={() => {
              setPage("search");
              setSearchResults([]);
              setSearchQuery("");
            }}
            disabled={page === "search"}
            style={navButtonStyle(page === "search")}
          >
            Search Results
          </button>
        </div>
        {page === "upload" && (
          <UploadPDF
            fileName={fileName}
            error={error}
            fileInputRef={fileInputRef}
            onFileChange={handleFileChange}
          />
        )}
        {page === "list" && <UploadedPDFList uploadedFiles={uploadedFiles} />}
        {page === "search" && (
          <div>
            <div style={{ marginBottom: 24, textAlign: "center" }}>
              <input
                type="text"
                value={searchQuery}
                onChange={e => setSearchQuery(e.target.value)}
                placeholder="Enter search query"
                style={{
                  padding: "8px 12px",
                  fontSize: 16,
                  borderRadius: 6,
                  border: "1px solid #cbd5e1",
                  marginRight: 12,
                  width: "60%"
                }}
              />
              <button
                onClick={() => fetchSearchResults(searchQuery)}
                style={{
                  ...navButtonStyle(false),
                  padding: "8px 20px"
                }}
                disabled={!searchQuery.trim()}
              >
                Search
              </button>
            </div>
            <SearchResultsTable results={searchResults} />
          </div>
        )}
      </div>
    </div>
  );
};

export default App;